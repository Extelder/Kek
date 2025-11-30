using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Transporting; // Channel.Reliable/Unreliable

/// <summary>
/// Обновлённый VoiceChat:
/// - 1 сетевой пакет = 1 аудиофрейм (FrameMs)
/// - seq (ushort) в начале пакета
/// - anti-burst: не собираем пачки фреймов в одном Update
/// - простая PLC (повтор последнего фрейма при пропуске)
/// - оставлена архитектура: PushToTalk/VoiceActivation, Proximity/Global
/// - плейсхолдеры для Opus (кодек можно вставить в Encode/Decode)
/// </summary>
public class VoiceChat : NetworkBehaviour
{

    [SerializeField] private MicrophoneManager _microphoneManager;
    public enum ChatType { Global, Proximity }
    public ChatType VoiceChatType = ChatType.Global;

    public enum DetectionType { PushToTalk, VoiceActivation }
    public DetectionType VoiceDetectionType = DetectionType.PushToTalk;

    [Header("General")]
    public bool Activated = true;
    public KeyCode PushToTalkKey = KeyCode.V;
    public AudioSource source;

    [Header("Proximity")]
    public float proximityRange = 10f;

    [Header("Voice Activation")]
    [Tooltip("Средняя амплитуда (0..1), выше которой включается передача.")]
    public float voiceActivationThreshold = 0.002f;

    [Header("Capture")]
    [Tooltip("Желаемая частота захвата. Если устройство не поддерживает — выберется ближайшая.")]
    public int sampleRate = 16000;

    /* ====== Параметры захвата/фреймов ====== */
    private const int FrameMs = 20;    // 20 ms frame
    private int frameSamples;          // samples per frame, рассчитывается по micRate
    private int micRate;

    /* ====== Микрофон захват ====== */
    private string deviceName;
    private AudioClip microphoneClip;
    private int position = 0;

    // повторы — чтобы не аллоцировать каждый раз
    private float[] audioFrame;
    private short[] pcm16Frame;
    private byte[] byteFrame; // содержит header + PCM16: [seqLo][seqHi][pcm...]
    private float[] sampleData; // для voice activation и индикатора
    private float[] micDataBuffer; // для GetMicInputVolume()

    // последовательный номер пакета на отправителе
    private ushort outSeq = 0;

    private bool canTalk = false;
    private bool previousCanTalk = false;
    private Coroutine transmitRoutine;

    /* ====== Playback (приём/воспроизведение) ====== */
    private const int playbackBufferSeconds = 1;
    private float[] playbackRing;
    private int writeHead = 0, readHead = 0, buffered = 0;
    private AudioClip playbackClip;
    private bool playbackStarted = false;
    private readonly object playbackLock = new object();
    private bool playbackInited = false;

    // Буферы для распаковки пришедшего пакета
    private float[] recvFloat;
    private short[] recvShort;

    // Кэш трансформов отправителей для proximity
    private readonly Dictionary<int, Transform> senderTransformCache = new Dictionary<int, Transform>();

    // Для простого PLC — последний принятый фрейм по отправителю
    private readonly Dictionary<int, short[]> lastFramePerSender = new Dictionary<int, short[]>();
    private readonly Dictionary<int, ushort> lastSeqPerSender = new Dictionary<int, ushort>();

    // Для индикатора уровня
    private float[] debugSampleBuffer;

    /* =========================
     *      Unity lifecycle
     * ========================= */
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!playbackInited)
            InitPlayback();

        if (!IsOwner)
            return;

        if (source == null)
            Debug.LogError("[VOICE] AudioSource not assigned!");

        deviceName = (Microphone.devices.Length > 0) ? Microphone.devices[0] : null;
        var mm =_microphoneManager;
        if (mm != null)
        {
            string fromManager = mm.GetCurrentDeviceName();
            if (!string.IsNullOrEmpty(fromManager))
                deviceName = fromManager;
        }

        if (string.IsNullOrEmpty(deviceName))
            Debug.LogError("[VOICE] No microphone device found!");

        SetupMicRate();
        AllocateCaptureBuffers();

        if (source != null)
            source.spatialBlend = (VoiceChatType == ChatType.Proximity) ? 1f : 0f;
    }

    private void Update()
    {
        if (!Activated)
            return;

        // следим за сменой устройства (у владельца)
        if (IsOwner)
        {
            var mm = _microphoneManager;
            if (mm != null)
            {
                string selected = mm.GetCurrentDeviceName();
                if (!string.IsNullOrEmpty(selected) && selected != deviceName)
                    UpdateMicrophone(selected);
            }
        }

        // режимы детекции — только у владельца
        if (IsOwner)
        {
            switch (VoiceDetectionType)
            {
                case DetectionType.PushToTalk:
                    canTalk = Input.GetKey(PushToTalkKey);
                    if (canTalk && microphoneClip == null)
                    {
                        StartMicrophone();
                        StartTalking();
                    }
                    else if (!canTalk && microphoneClip != null)
                    {
                        StopTalking();
                        StopMicrophone();
                    }
                    break;

                case DetectionType.VoiceActivation:
                    if (microphoneClip == null)
                        StartMicrophone();
                    canTalk = IsVoiceActivated();
                    break;
            }

            if (!previousCanTalk && canTalk)
                StartTalking();
            if (previousCanTalk && !canTalk)
                StopTalking();

            previousCanTalk = canTalk;
        }

        // обновление spatialBlend у всех
        if (source != null)
            source.spatialBlend = (VoiceChatType == ChatType.Proximity) ? 1f : 0f;
    }

    /* =========================
     *      Микрофон / буферы
     * ========================= */
    private void SetupMicRate()
    {
        micRate = sampleRate;
        if (string.IsNullOrEmpty(deviceName))
            return;

        Microphone.GetDeviceCaps(deviceName, out int min, out int max);
        if (max != 0)
        {
            if (sampleRate < min) micRate = min;
            else if (sampleRate > max) micRate = max;
            else micRate = sampleRate;
        }

        frameSamples = Mathf.Max(1, micRate * FrameMs / 1000);
    }

    private void AllocateCaptureBuffers()
    {
        frameSamples = Mathf.Max(1, micRate * FrameMs / 1000);
        audioFrame    = new float[frameSamples];
        pcm16Frame    = new short[frameSamples];
        // header = 2 bytes seq; then pcm16Frame.Length*2 bytes
        byteFrame     = new byte[2 + frameSamples * 2];
        sampleData    = new float[frameSamples];
        micDataBuffer = new float[frameSamples];
        debugSampleBuffer = new float[frameSamples];
    }

    private void StartMicrophone()
    {
        if (string.IsNullOrEmpty(deviceName))
            return;

        SetupMicRate();
        AllocateCaptureBuffers();

        position = 0;
        microphoneClip = Microphone.Start(deviceName, true, 10, micRate);
    }

    private void StopMicrophone()
    {
        if (string.IsNullOrEmpty(deviceName))
            return;

        Microphone.End(deviceName);
        microphoneClip = null;
    }

    private void UpdateMicrophone(string newDevice)
    {
        if (!string.IsNullOrEmpty(deviceName))
        {
            StopTalking();
            StopMicrophone();
        }

        deviceName = newDevice;
        if (IsOwner && canTalk)
        {
            StartMicrophone();
            StartTalking();
        }
    }

    /* =========================
     *      Передача
     * ========================= */
    private void StartTalking()
    {
        if (!IsOwner) return;
        if (string.IsNullOrEmpty(deviceName)) return;
        if (transmitRoutine != null) StopCoroutine(transmitRoutine);
        transmitRoutine = StartCoroutine(TransmitVoice());
    }

    private void StopTalking()
    {
        if (transmitRoutine != null)
        {
            StopCoroutine(transmitRoutine);
            transmitRoutine = null;
        }
    }

    // Корутин: отправляем ровно один фрейм за итерацию -> предотвращаем burst'ы
    private IEnumerator TransmitVoice()
    {
        // safety
        if (microphoneClip == null || string.IsNullOrEmpty(deviceName))
            yield break;

        while (canTalk)
        {
            if (microphoneClip == null)
                yield break;

            int micPos = Microphone.GetPosition(deviceName);
            int available = (micPos - position + microphoneClip.samples) % microphoneClip.samples;

            // Если есть хотя бы один полный фрейм — возьмём только один за итерацию
            if (available >= frameSamples)
            {
                microphoneClip.GetData(audioFrame, position);
                position = (position + frameSamples) % microphoneClip.samples;

                // PCM16
                for (int i = 0; i < frameSamples; i++)
                {
                    float v = Mathf.Clamp(audioFrame[i], -1f, 1f);
                    pcm16Frame[i] = (short)(v * short.MaxValue);
                }

                // Записываем seq (2 байта little-endian) + PCM16
                byteFrame[0] = (byte)(outSeq & 0xFF);
                byteFrame[1] = (byte)((outSeq >> 8) & 0xFF);
                System.Buffer.BlockCopy(pcm16Frame, 0, byteFrame, 2, frameSamples * 2);

                // Отправка через сервер->наблюдатели (Unreliable)
                TransmitAudioServerRpc(byteFrame, Channel.Unreliable);

                outSeq++;
            }

            // ждать следующий кадр/фрейм — yield null достаточно
            yield return null;
        }
    }

    private bool IsVoiceActivated()
    {
        if (microphoneClip == null || string.IsNullOrEmpty(deviceName))
            return false;

        int micPosition = Microphone.GetPosition(deviceName);
        int start = micPosition - frameSamples;
        if (start < 0)
            return false;

        microphoneClip.GetData(sampleData, start);

        float sum = 0f;
        for (int i = 0; i < sampleData.Length; i++)
            sum += Mathf.Abs(sampleData[i]);

        float avg = sum / sampleData.Length;
        return avg > voiceActivationThreshold;
    }

    /* =========================
     *        RPC / сетка
     * ========================= */
    [ServerRpc(RequireOwnership = false, RunLocally = false)]
    private void TransmitAudioServerRpc(byte[] audioPacket, Channel channel = Channel.Unreliable, NetworkConnection sender = null)
    {
        int senderId = (sender != null) ? sender.ClientId : -1;
        TransmitAudioObserversRpc(audioPacket, senderId, channel);
    }

    [ObserversRpc(BufferLast = false)]
    private void TransmitAudioObserversRpc(byte[] audioPacket, int senderClientId, Channel channel = Channel.Unreliable)
    {
        // не воспроизводим свой собственный звук (локальный клиент)
        if (NetworkManager != null &&
            NetworkManager.ClientManager != null &&
            NetworkManager.ClientManager.Connection != null &&
            senderClientId == NetworkManager.ClientManager.Connection.ClientId)
            return;

        PlayReceivedAudio(audioPacket, senderClientId);
    }

    /* =========================
     *      Приём/воспроизведение
     * ========================= */
    private void InitPlayback()
    {
        if (source == null)
            source = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        if (micRate <= 0) micRate = Mathf.Max(8000, sampleRate);
        frameSamples = Mathf.Max(1, micRate * FrameMs / 1000);

        int ringSize = Mathf.Max(micRate * playbackBufferSeconds, frameSamples * 6);
        playbackRing = new float[ringSize];
        writeHead = readHead = buffered = 0;

        playbackClip = AudioClip.Create("VoiceStream", ringSize, 1, micRate, true, OnAudioRead);
        source.clip = playbackClip;
        source.loop = true;
        source.dopplerLevel = 0f;
        source.spatialBlend = (VoiceChatType == ChatType.Proximity) ? 1f : 0f;
        source.playOnAwake = false;

        playbackStarted = false;
        playbackInited = true;
    }

    private void PlayReceivedAudio(byte[] audioPacket, int senderClientId)
    {
        if (source == null)
        {
            Debug.LogError("[VOICE] AudioSource not assigned!");
            return;
        }

        if (!playbackInited || playbackRing == null || playbackRing.Length == 0)
            InitPlayback();

        if (VoiceChatType == ChatType.Proximity)
        {
            source.maxDistance = proximityRange;
            var senderTf = GetSenderTransformCached(senderClientId);
            if (senderTf != null)
            {
                float dist = Vector3.Distance(transform.position, senderTf.position);
                if (dist > proximityRange)
                    return;
                // Для 3D-позиционирования можно создать отдельный AudioSource на позицию senderTf
            }
        }

        // Минимальная валидация
        if (audioPacket == null || audioPacket.Length < 2)
            return;

        // Выделим/рассчитаем размеры
        int payloadBytes = audioPacket.Length - 2; // первый 2 байта — seq
        int samples = payloadBytes / 2; // PCM16 -> short -> samples

        if (recvShort == null || recvShort.Length != samples) recvShort = new short[samples];
        if (recvFloat == null || recvFloat.Length != samples) recvFloat = new float[samples];

        // Извлекаем seq
        ushort seq = (ushort)(audioPacket[0] | (audioPacket[1] << 8));

        // Распаковка PCM16 (начиная с offset 2)
        System.Buffer.BlockCopy(audioPacket, 2, recvShort, 0, payloadBytes);
        for (int i = 0; i < samples; i++)
            recvFloat[i] = recvShort[i] / (float)short.MaxValue;

        lock (playbackLock)
        {
            // Простая потеря-пакетов обработка:
            // если пришёл пакет с seq, который НЕ следующий после lastSeq, считаем что некоторые потеряны.
            bool hadLost = false;
            if (senderClientId >= 0)
            {
                if (lastSeqPerSender.TryGetValue(senderClientId, out ushort lastSeq))
                {
                    ushort expected = (ushort)(lastSeq + 1);
                    if (seq != expected)
                    {
                        // потеря(и) — повторим последний фрейм один раз за каждый пропуск (ограничим до 3 повторов чтобы не застрять)
                        int gap = (seq - expected + 65536) % 65536;
                        int repeats = Mathf.Clamp(gap, 0, 3);
                        if (lastFramePerSender.TryGetValue(senderClientId, out short[] lastFrame) && lastFrame != null)
                        {
                            for (int r = 0; r < repeats; r++)
                            {
                                for (int i = 0; i < lastFrame.Length; i++)
                                {
                                    float v = lastFrame[i] / (float)short.MaxValue;
                                    playbackRing[writeHead] = v;
                                    writeHead = (writeHead + 1) % playbackRing.Length;
                                    if (buffered < playbackRing.Length) buffered++;
                                    else readHead = (readHead + 1) % playbackRing.Length;
                                }
                            }
                            hadLost = repeats > 0;
                        }
                    }
                }
            }

            // Записываем текущий фрейм в playbackRing
            for (int i = 0; i < samples; i++)
            {
                playbackRing[writeHead] = recvFloat[i];
                writeHead = (writeHead + 1) % playbackRing.Length;
                if (buffered < playbackRing.Length) buffered++;
                else readHead = (readHead + 1) % playbackRing.Length; // при переполнении дроп старого
            }

            // Сохраняем последний фрейм (для PLC) в формате PCM16 (short[])
            if (senderClientId >= 0)
            {
                if (!lastFramePerSender.TryGetValue(senderClientId, out short[] dst) || dst == null || dst.Length != samples)
                {
                    dst = new short[samples];
                    lastFramePerSender[senderClientId] = dst;
                }
                System.Buffer.BlockCopy(audioPacket, 2, dst, 0, samples * 2);
                lastSeqPerSender[senderClientId] = seq;
            }

            // Запуск воспроизведения после небольшой подушки
            if (!playbackStarted && buffered >= frameSamples * 3)
            {
                source.Play();
                playbackStarted = true;
            }
        }
    }

    private void OnAudioRead(float[] data)
    {
        lock (playbackLock)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (buffered > 0)
                {
                    data[i] = playbackRing[readHead];
                    readHead = (readHead + 1) % playbackRing.Length;
                    buffered--;
                }
                else
                {
                    data[i] = 0f; // тишина при пустом буфере
                }
            }
        }
    }

    private void OnAudioSetPosition(int newPosition) { }

    /* =========================
     *        Утилиты
     * ========================= */
    private Transform GetSenderTransformCached(int clientId)
    {
        if (clientId < 0) return null;

        if (senderTransformCache.TryGetValue(clientId, out var tf) && tf != null)
            return tf;

        var objs = FindObjectsOfType<NetworkObject>();
        foreach (var no in objs)
        {
            if (no.Owner != null && no.Owner.ClientId == clientId)
            {
                senderTransformCache[clientId] = no.transform;
                return no.transform;
            }
        }
        return null;
    }

    // Индикатор уровня входа (0..1)
    public float GetMicInputVolume()
    {
        if (!IsOwner || microphoneClip == null || string.IsNullOrEmpty(deviceName))
            return 0f;

        int micPosition = Microphone.GetPosition(deviceName);
        int start = micPosition - frameSamples;
        if (start < 0) return 0f;

        microphoneClip.GetData(micDataBuffer, start);

        float sum = 0f;
        for (int i = 0; i < micDataBuffer.Length; i++)
            sum += micDataBuffer[i] * micDataBuffer[i];

        float rms = Mathf.Sqrt(sum / micDataBuffer.Length);
        return Mathf.Clamp01(rms * 50f);
    }

    /* =========================
     *      Opus / Encoding (placeholders)
     * ========================= */
    // Здесь можно подключить Opus: Encode(float[] / short[]) -> byte[] меньшего размера
    // и на приёме Decode(byte[]) -> short[].
    // Если интегрируешь Opus, замени момент, где мы формируем byteFrame и где распаковываем на приёме.
    // Примечание: подпись RPC остаётся byte[], просто payload станет уже закодированным.
}
