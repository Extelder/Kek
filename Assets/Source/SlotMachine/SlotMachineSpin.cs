using System;
using System.Collections;
using UnityEngine;
using FishNet.Object;
using Random = UnityEngine.Random;

public class SlotMachineSpin : NetworkBehaviour
{
    [Header("Reels")]
    [SerializeField] private Transform[] reels = new Transform[3];
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioSource _audioBaraban;
    [SerializeField] private AudioSource _audioStop;
    [SerializeField] private AudioClip[] _audioStopClip;
    [SerializeField] private NetWorkAnimatorSynchronize _netWorkAnimator;

    public enum RotationAxis { X, Y, Z }

    [Header("Rotation")]
    [SerializeField] private RotationAxis axis = RotationAxis.Z;
    [SerializeField] private bool invert = false;
    [SerializeField] private int symbolsPerReel = 11;

    [Header("Spin Tuning")]
    [SerializeField] private float spinDuration = 3.0f;      // общий момент финиша
    [SerializeField] private float stopStagger = 0.35f;      // старт по очереди, но финиш общий
    [SerializeField] private Vector2 extraTurnsRange = new Vector2(3, 6); // мин..макс целых оборотов
    [SerializeField] private AnimationCurve decelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // (1,1)!

    [Header("Segments (non-uniform)")]
    [SerializeField] private float angleZeroOffsetDeg = 0f;  // общий сдвиг «верха»
    [SerializeField] private float[] segmentMidAnglesDeg;    // длиной = symbolsPerReel

    // локальные состояния
    private Quaternion[] _baseRot;   // базовая локальная ориентация каждого рила
    private float[] _reelAngle;      // наш «накопительный» угол каждого рила (0..360)
    public bool spinning;

    private void Start()
    {
        _audioBaraban.Play();
        _audioBaraban.Pause();
    }

    private void Awake()
    {
        if (reels == null || reels.Length != 3)
            Debug.LogError("Assign exactly 3 reels.");

        if (segmentMidAnglesDeg == null || segmentMidAnglesDeg.Length != symbolsPerReel)
            Debug.LogError("SegmentMidAnglesDeg length must equal SymbolsPerReel.");

        _baseRot = new Quaternion[reels.Length];
        _reelAngle = new float[reels.Length];
        for (int i = 0; i < reels.Length; i++)
        {
            _baseRot[i] = reels[i].localRotation;
            _reelAngle[i] = 0f; // считаем текущий угол 0 относительно базы
        }
    }

    // ===== ПУСК =====
    public void StartSpin()
    {
        if (spinning) return;
        if (!IsServer) { ServerSpinRequest(); return; }
        // если мы сервер/хост — сразу запускаем
        ServerSpinStart();
    }

    // Клиент просит сервер запустить
    [ServerRpc(RequireOwnership = false)]
    private void ServerSpinRequest() => ServerSpinStart();

    // Сервер генерит параметры и шлёт всем наблюдателям
    private void ServerSpinStart()
    {
        if (spinning) return;
        spinning = true;
                    
        _netWorkAnimator.PlayAnim("Spin");

        int[] targets = new int[3];
        int[] extras  = new int[3];
        for (int i = 0; i < 3; i++)
        {
            targets[i] = Random.Range(0, symbolsPerReel);
            // целые обороты
            int minT = Mathf.Max(0, Mathf.CeilToInt(extraTurnsRange.x));
            int maxT = Mathf.Max(minT, Mathf.CeilToInt(extraTurnsRange.y));
            extras[i]  = Random.Range(minT, maxT + 1);
        }

        RpcSpinAll(targets, extras, spinDuration, stopStagger);
        // серверу тоже нужно крутить локально – RPC прилетит и ему
    }

    [ObserversRpc(BufferLast = false)]
    private void RpcSpinAll(int[] targetIdx, int[] extraTurns, float duration, float stagger)
    {
        if (_audio) _audio.Play();

        for (int i = 0; i < reels.Length; i++)
        {
            float delay = i * stagger;          // по очереди
            float durI  = duration;             // все крутятся одинаковое время
            StartCoroutine(SpinSingleRoutine(i, targetIdx[i], extraTurns[i], durI, delay));
        }

        // ждать окончания ПОСЛЕДНЕГО барабана
        float total = duration + (reels.Length - 1) * stagger + 0.05f;
        StartCoroutine(ClearAndCheckAfter(total));
    }
    private IEnumerator ClearAndCheckAfter(float t)
    {
        yield return new WaitForSeconds(t);
        spinning = false;
        CheckResults();                          // лог по окончанию всех
    }

    private IEnumerator ClearSpinningAfter(float t)
    {
        yield return new WaitForSeconds(t);
        spinning = false;
    }

    // ===== КОРУТИНА ЛОКАЛЬНОГО ВРАЩЕНИЯ (НЕ RPC!) =====
    private IEnumerator SpinSingleRoutine(int reelIndex, int targetIndex, int extraTurnsInt, float duration, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        _audioBaraban.UnPause();

        Transform reel = reels[reelIndex];
        Vector3 axisVec = (axis == RotationAxis.X) ? Vector3.right :
                          (axis == RotationAxis.Y) ? Vector3.up    : Vector3.forward;
        float sign = invert ? -1f : 1f;

        // стартуем от нашего накопительного угла
        float startAngle = _reelAngle[reelIndex];
        Quaternion baseRot = _baseRot[reelIndex];

        // целевой центр сегмента (общий оффсет + midAngle)
        float mid = segmentMidAnglesDeg[targetIndex];
        float target = Mathf.Repeat(angleZeroOffsetDeg + mid, 360f);

        // идём к target строго в сторону sign
        float startMod = Mathf.Repeat(startAngle, 360f);
        float deltaToTarget = Mathf.DeltaAngle(startMod, target); // [-180..180]
        if (Mathf.Sign(deltaToTarget) != Mathf.Sign(sign))
            deltaToTarget += 360f * sign;

        float totalTravel = deltaToTarget + sign * 360f * Mathf.Max(0, extraTurnsInt);
        float endAngle = startAngle + totalTravel;   // конечный абсолютный угол
        bool preStopPlayed = false;
        const float preStopWindow = 0.12f;

        // плавное торможение до endAngle — без пост-снапа
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float eased = decelCurve.Evaluate(Mathf.Clamp01(t / duration)); // последний ключ (1,1)!
            float cur = Mathf.Lerp(startAngle, endAngle, eased);            // линейный по абсолютному углу
            reel.localRotation = baseRot * Quaternion.AngleAxis(cur, axisVec);
            if (!preStopPlayed && duration - t <= preStopWindow)
            {
                _audioStop.clip = _audioStopClip[Random.Range(0, _audioStopClip.Length)];
                _audioStop.Play();
            }
            yield return null;
        }

        reel.localRotation = baseRot * Quaternion.AngleAxis(endAngle, axisVec);
        _reelAngle[reelIndex] = Mathf.Repeat(endAngle, 360f);
        _audioBaraban.Pause();
    }

    // ===== утилиты =====
    private float GetAngle(Transform t)
    {
        var e = t.localEulerAngles;
        return axis == RotationAxis.X ? e.x :
               axis == RotationAxis.Y ? e.y : e.z;
    }

    private void CheckResults()
    {
        // вызывать по желанию после общего финиша; тут пример локального вывода
        // можно считать на сервере и шлёть сообщение/награду
    }
}
