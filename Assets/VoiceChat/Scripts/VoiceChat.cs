using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Transporting; // Channel.Reliable/Unreliable

public class VoiceChat : NetworkBehaviour
{
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

    /* ====== Передача (захват микрофона) ====== */
    private string deviceName;
    private AudioClip microphoneClip;
    private int micRate;               // реальная частота записи
    private int position;              // позиция чтения из кольца Unity-микрофона

    private const int FrameMs = 20;    // 20 мс кадр
    private int frameSamples;          // вычисляется из micRate

    private float[] audioFrame;
    private short[] pcm16Frame;
    private byte[] byteFrame;

    private bool canTalk = false;
    private bool previousCanTalk = false;
    private Coroutine transmitRoutine;

    /* ====== Приём (непрерывный стрим) ====== */
    private const int playbackBufferSeconds = 1; // размер кольца приёма
    private float[] playbackRing;
    private int writeHead, readHead, buffered;
    private AudioClip playbackClip;
    private bool playbackStarted;
    private readonly object playbackLock = new object();
    private bool playbackInited;

    // Временные буферы для распаковки пришедшего пакета
    private float[] recvFloat;
    private short[] recvShort;

    // Кэш трансформов отправителей для проксими-чата
    private readonly Dictionary<int, Transform> senderTransformCache = new Dictionary<int, Transform>();

    // Служебные буферы под Voice Activation и визуал индикатор
    private float[] sampleData;
    private float[] micDataBuffer;

    /* =========================
     *      Unity lifecycle
     * ========================= */
    public override void OnStartClient()
    {
        base.OnStartClient();

        // Приём нужно инициализировать у всех (и у владельца, и у наблюдателей)
        if (!playbackInited)
            InitPlayback();

        // Ниже — только логика захвата/отправки у владельца
        if (!base.IsOwner)
            return;

        if (source == null)
            Debug.LogError("[VOICE] AudioSource not assigned!");

        // Выбор устройства: если есть MicrophoneManager — используем его, иначе берём первое
        deviceName = (Microphone.devices.Length > 0) ? Microphone.devices[0] : null;
        var mm = FindObjectOfType<MicrophoneManager>();
        if (mm != null)
        {
            string fromManager = mm.GetCurrentDeviceName();
            if (!string.IsNullOrEmpty(fromManager))
                deviceName = fromManager;
        }

        if (string.IsNullOrEmpty(deviceName))
            Debug.LogError("[VOICE] No microphone device found!");

        // Подстроим частоту и размеры кадров
        SetupMicRate();
        frameSamples = Mathf.Max(1, micRate * FrameMs / 1000);

        audioFrame    = new float[frameSamples];
        pcm16Frame    = new short[frameSamples];
        byteFrame     = new byte[frameSamples * 2];
        sampleData    = new float[frameSamples];
        micDataBuffer = new float[frameSamples];

        if (source != null)
            source.spatialBlend = (VoiceChatType == ChatType.Proximity) ? 1f : 0f;
    }

    private void Update()
    {
        if (!Activated)
            return;

        // Отслеживаем смену микрофона через менеджер (если есть) — только у владельца
        if (base.IsOwner)
        {
            var mm = FindObjectOfType<MicrophoneManager>();
            if (mm != null)
            {
                string selected = mm.GetCurrentDeviceName();
                if (!string.IsNullOrEmpty(selected) && selected != deviceName)
                    UpdateMicrophone(selected);
            }
        }

        // Режимы детекции — только у владельца, кто отправляет
        if (base.IsOwner)
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

            // Фронты включения/выключения передачи
            if (!previousCanTalk && canTalk)
                StartTalking();
            if (previousCanTalk && !canTalk)
                StopTalking();

            previousCanTalk = canTalk;
        }

        // Поддерживаем пространственность у всех
        if (source != null)
            source.spatialBlend = (VoiceChatType == ChatType.Proximity) ? 1f : 0f;
    }

    /* =========================
     *      МИКРОФОН (захват)
     * ========================= */
    private void SetupMicRate()
    {
        micRate = sampleRate;
        if (string.IsNullOrEmpty(deviceName))
            return;

        // У некоторых драйверов Unity возвращает 0/0 — трактуем как «без ограничений»
        Microphone.GetDeviceCaps(deviceName, out int min, out int max);
        if (max != 0)
        {
            if (sampleRate < min) micRate = min;
            else if (sampleRate > max) micRate = max;
            else micRate = sampleRate;
        }
    }

    private void StartMicrophone()
    {
        if (string.IsNullOrEmpty(deviceName))
            return;

        SetupMicRate();
        frameSamples = Mathf.Max(1, micRate * FrameMs / 1000);

        audioFrame    = new float[frameSamples];
        pcm16Frame    = new short[frameSamples];
        byteFrame     = new byte[frameSamples * 2];
        sampleData    = new float[frameSamples];
        micDataBuffer = new float[frameSamples];

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

    private void UpdateMicrophone(string newDeviceName)
    {
        if (!string.IsNullOrEmpty(deviceName))
        {
            StopTalking();
            StopMicrophone();
        }

        deviceName = newDeviceName;

        if (base.IsOwner && canTalk)
        {
            StartMicrophone();
            StartTalking();
        }
    }

    /* =========================
     *         ПЕРЕДАЧА
     * ========================= */
    private void StartTalking()
    {
        if (!base.IsOwner)
            return;

        if (string.IsNullOrEmpty(deviceName))
            return;

        if (transmitRoutine != null)
            StopCoroutine(transmitRoutine);
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

    private IEnumerator TransmitVoice()
    {
        while (canTalk)
        {
            if (microphoneClip == null)
                yield break;

            int micPos = Microphone.GetPosition(deviceName);
            int available = (micPos - position + microphoneClip.samples) % microphoneClip.samples;

            // Отправляем пакетами по frameSamples
            while (available >= frameSamples)
            {
                microphoneClip.GetData(audioFrame, position);
                position = (position + frameSamples) % microphoneClip.samples;
                available -= frameSamples;

                // PCM16
                for (int i = 0; i < frameSamples; i++)
                {
                    float v = Mathf.Clamp(audioFrame[i], -1f, 1f);
                    pcm16Frame[i] = (short)(v * short.MaxValue);
                }
                System.Buffer.BlockCopy(pcm16Frame, 0, byteFrame, 0, byteFrame.Length);

                // Ненадёжный канал — ниже задержка, нет head-of-line
                TransmitAudioServerRpc(byteFrame, Channel.Unreliable);
            }

            yield return null; // в следующий кадр
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
     *            RPC
     * ========================= */
    // Примечание: в FishNet можно добавить параметр Channel в сигнатуру RPC.
    [ServerRpc(RequireOwnership = false, RunLocally = false)]
    private void TransmitAudioServerRpc(byte[] audioPacket, Channel channel = Channel.Unreliable, NetworkConnection sender = null)
    {
        int senderId = (sender != null) ? sender.ClientId : -1;
        TransmitAudioObserversRpc(audioPacket, senderId, channel);
    }

    [ObserversRpc(BufferLast = false)]
    private void TransmitAudioObserversRpc(byte[] audioPacket, int senderClientId, Channel channel = Channel.Unreliable)
    {
        // Не играем собственный звук на том же коннекте
        if (NetworkManager != null &&
            NetworkManager.ClientManager != null &&
            NetworkManager.ClientManager.Connection != null &&
            senderClientId == NetworkManager.ClientManager.Connection.ClientId)
            return;

        PlayReceivedAudio(audioPacket, senderClientId);
    }

    /* =========================
     *      ПРИЁМ/ВОСПРОИЗВЕДЕНИЕ
     * ========================= */
    private void InitPlayback()
    {
        // AudioSource обязателен для воспроизведения; если не задан — создадим
        if (source == null)
            source = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        // micRate может ещё не быть выставлен у наблюдателя — используем заданный sampleRate
        if (micRate <= 0) micRate = Mathf.Max(8000, sampleRate);

        frameSamples = Mathf.Max(1, micRate * FrameMs / 1000);

        int ringSize = Mathf.Max(micRate * playbackBufferSeconds, frameSamples * 6); // небольшой запас поверх окна

        playbackRing = new float[ringSize];
        writeHead = readHead = buffered = 0;

        // Создаём стримовый клип; используем только OnAudioRead
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

        // На случай гонки: если приём ещё не инициализирован — инициализируем лениво
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
                // Для полноценного 3D позиционирования можно двигать отдельный AudioSource к senderTf.position
            }
        }

        int samples = audioPacket.Length / 2;
        if (recvShort == null || recvShort.Length != samples) recvShort = new short[samples];
        if (recvFloat == null || recvFloat.Length != samples) recvFloat = new float[samples];

        System.Buffer.BlockCopy(audioPacket, 0, recvShort, 0, audioPacket.Length);
        for (int i = 0; i < samples; i++)
            recvFloat[i] = recvShort[i] / (float)short.MaxValue;

        lock (playbackLock)
        {
            // Страховка на всякий
            if (playbackRing == null || playbackRing.Length == 0)
                return;

            for (int i = 0; i < samples; i++)
            {
                playbackRing[writeHead] = recvFloat[i];
                writeHead = (writeHead + 1) % playbackRing.Length;
                if (buffered < playbackRing.Length) buffered++;
                else readHead = (readHead + 1) % playbackRing.Length; // переполнение — дроп старого
            }

            // Небольшая подушка (3 кадра) перед стартом — сглаживает сетевой джиттер
            if (!playbackStarted && buffered >= frameSamples * 3)
            {
                source.Play();
                playbackStarted = true;
            }
        }
    }

    // PCM callback из AudioClip.Create — Unity спросит данные для проигрывания
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
                    data[i] = 0f; // тишина, если буфер пуст (сетевые провалы)
                }
            }
        }
    }

    // Не используется, но сигнатура допустима, если хочешь следить за позициями
    private void OnAudioSetPosition(int newPosition) { }

    /* =========================
     *           Утилиты
     * ========================= */
    private Transform GetSenderTransformCached(int clientId)
    {
        if (clientId < 0)
            return null;

        if (senderTransformCache.TryGetValue(clientId, out var tf) && tf != null)
            return tf;

        // Фоллбек-поиск (лучше заменить на свой реестр игроков)
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

    // Для отладочного индикатора уровня входа (0..1) — можно привязать к UI
    public float GetMicInputVolume()
    {
        if (!base.IsOwner || microphoneClip == null || string.IsNullOrEmpty(deviceName))
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
}

/*
 * Заметки:
 * - Если используешь свой MicrophoneManager, он должен уметь вернуть текущее deviceName.
 * - Канал RPC — Unreliable (UDP-поведение) для голоса; так мы избегаем head-of-line блокировок.
 * - Хоть мы и используем 1-сек. кольцо, стартуем воспроизведение после ~60 мс (3 кадра по 20 мс).
 * - Для продакшена добавь Opus (20 ms frames, VBR, PLC) и sequence-номера пакетов с адаптивной джиттер-подушкой.
 */
