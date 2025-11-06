using System.Collections;
using UnityEngine;

public class SlotMachineInteract : Item
{
    [SerializeField] private NetWorkAnimatorSynchronize _netWorkAnimator;
    [SerializeField] private MixSoundAndPlay _audio;

    [Header("Reels")]
    [SerializeField] private Transform[] reels = new Transform[3];

    public enum RotationAxis { X, Y, Z }

    [Header("Rotation")]
    [SerializeField] private RotationAxis axis = RotationAxis.Z;
    [SerializeField] private bool invert = false;              // инвертировать направление
    [SerializeField] private int symbolsPerReel = 11;          // у тебя 11

    [Header("Spin Tuning")]
    [SerializeField] private float spinDuration = 3.0f;        // общий момент финиша
    [SerializeField] private float stopStagger = 0.35f;        // сдвиг старта (финиш всё равно общий)
    [SerializeField] private Vector2 extraTurnsRange = new Vector2(3, 6);
    [SerializeField] private AnimationCurve decelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Segments (non-uniform)")]
    [Tooltip("Смещение нулевой метки (если верх не на 0°).")]
    [SerializeField] private float angleZeroOffsetDeg = 0f;

    [Tooltip("Если оставить пустым — сгенерим с одним узким сегментом.")]
    [SerializeField] private float[] segmentMidAnglesDeg;

    [Tooltip("Индекс узкого сегмента (или -1, если не нужен автоподсчёт).")]
    [SerializeField] private int narrowSegmentIndex = 0;

    [Tooltip("Во сколько раз уже узкий сегмент (0.67 ≈ уже на треть).")]
    [SerializeField, Range(0.2f, 0.95f)] private float narrowFactor = 0.67f;

    private int[] result = new int[3];
    private bool spinning;

    private void Awake()
    {
        if (segmentMidAnglesDeg == null || segmentMidAnglesDeg.Length != symbolsPerReel)
            segmentMidAnglesDeg = GenerateMidAngles(symbolsPerReel, narrowSegmentIndex, narrowFactor, angleZeroOffsetDeg);
    }

    public override void Interact()
    {
        _netWorkAnimator.SetAnimatorBool("Spin", !_netWorkAnimator.Animator.GetBool("Spin"));
        _audio.MixOnServer();
        StartSpin();
    }

    public void StartSpin()
    {
        if (spinning) return;
        if (reels == null || reels.Length < 3)
        {
            Debug.LogWarning("Assign 3 reels in inspector.");
            return;
        }
        _netWorkAnimator.StartCoroutine(SpinRoutine()); // ВАЖНО: запускать на этом компоненте
    }

    private IEnumerator SpinRoutine()
    {
        spinning = true;

        for (int i = 0; i < 3; i++)
            result[i] = Random.Range(0, symbolsPerReel);

        for (int i = 0; i < 3; i++)
        {
            float delayStart = i * stopStagger;                       // можно 0, если без сдвига
            float durationI  = Mathf.Max(0.2f, spinDuration - delayStart);
            _netWorkAnimator.StartCoroutine(SpinSingle(reels[i], result[i], durationI, delayStart));
        }

        yield return new WaitForSeconds(spinDuration + 0.05f);

        CheckResults();
        spinning = false;
    }

    // === helpers для работы с нужной осью ===
    float GetAngle(Transform t)
    {
        var e = t.localEulerAngles;
        switch (axis)
        {
            case RotationAxis.X: return e.x;
            case RotationAxis.Y: return e.y;
            default:             return e.z;
        }
    }

    void SetAngle(Transform t, float a)
    {
        var e = t.localEulerAngles;
        switch (axis)
        {
            case RotationAxis.X: e.x = a; break;
            case RotationAxis.Y: e.y = a; break;
            default:             e.z = a; break;
        }
        t.localEulerAngles = e;
    }

    // === сегменты ===
    float GetTargetAngleDeg(int targetIndex)
    {
        if (segmentMidAnglesDeg != null && segmentMidAnglesDeg.Length == symbolsPerReel)
            return Mathf.Repeat(segmentMidAnglesDeg[targetIndex] + angleZeroOffsetDeg, 360f);

        // запасной случай: равномерная сетка
        float step = 360f / symbolsPerReel;
        return Mathf.Repeat(-targetIndex * step + angleZeroOffsetDeg, 360f);
    }

    static float[] GenerateMidAngles(int count, int narrowIndex, float narrowFactor, float zeroOffset)
    {
        float[] widths = new float[count];
        for (int i = 0; i < count; i++) widths[i] = 1f;
        if (narrowIndex >= 0 && narrowIndex < count)
            widths[narrowIndex] = Mathf.Clamp(narrowFactor, 0.2f, 0.95f);

        float sum = 0f;
        for (int i = 0; i < count; i++) sum += widths[i];

        float[] mids = new float[count];
        float accum = 0f;
        for (int i = 0; i < count; i++)
        {
            float segAngle = widths[i] / sum * 360f;
            float mid = accum + segAngle * 0.5f;
            mids[i] = Mathf.Repeat(mid + zeroOffset, 360f);
            accum += segAngle;
        }
        return mids;
    }

    // === спин без рывков ===
    private IEnumerator SpinSingle(Transform reel, int targetIndex, float duration, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        float sign = invert ? -1f : 1f;

        // стартовый угол по выбранной оси (0..360)
        float start = GetAngle(reel);

        // целевой мидпоинт сегмента
        float target = GetTargetAngleDeg(targetIndex); // 0..360

        // идём к target строго в сторону sign
        float startMod = Mathf.Repeat(start, 360f);
        float deltaToTarget = Mathf.DeltaAngle(startMod, target); // [-180..180] кратчайший
        if (Mathf.Sign(deltaToTarget) != Mathf.Sign(sign))
            deltaToTarget += 360f * sign;

        // добавим несколько оборотов в ту же сторону
        float extraTurns = Random.Range(extraTurnsRange.x, extraTurnsRange.y);
        float totalTravel = deltaToTarget + sign * 360f * extraTurns;

        // конечный абсолютный угол (в градусах, может быть >360/ <0 — это ок)
        float endAbs = start + totalTravel;

        // основная фаза: плавное замедление — в t==duration мы уже на endAbs (никаких пост-снапoв)
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float eased = decelCurve.Evaluate(Mathf.Clamp01(t / duration)); // 0..1, ease-out
            float a = Mathf.Lerp(start, endAbs, eased);                      // ЛИНЕЙНЫЙ Lerp по абсолютным углам (НЕ LerpAngle)
            SetAngle(reel, a);
            yield return null;
        }

        // финальная фиксация (та же endAbs)
        SetAngle(reel, endAbs);
    }

    private void CheckResults()
    {
        bool allEqual = (result[0] == result[1]) && (result[1] == result[2]);
        bool twoEqual =
            (result[0] == result[1]) ||
            (result[0] == result[2]) ||
            (result[1] == result[2]);

        if (allEqual)      Debug.Log($"BIG WIN!! [{result[0]} {result[1]} {result[2]}]");
        else if (twoEqual) Debug.Log($"Small win: two matched [{result[0]} {result[1]} {result[2]}]");
        else               Debug.Log($"No win [{result[0]} {result[1]} {result[2]}]");
    }
}
