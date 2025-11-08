using System;
using System.Collections;
using FishNet.Demo.AdditiveScenes;
using UnityEngine;
using FishNet.Object;
using Random = UnityEngine.Random;

public class SlotMachineSpin : NetworkBehaviour
{
    [Header("Reels")] [SerializeField] private Transform[] reels = new Transform[3];
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioSource _audioBaraban;
    [SerializeField] private AudioSource _audioStop;
    [SerializeField] private AudioClip[] _audioStopClip;
    [SerializeField] private NetWorkAnimatorSynchronize _netWorkAnimator;
    [Header("Money")]
    [SerializeField] private int _price;
    [SerializeField] private int _priceBigWin;
    [SerializeField] private int _priceWin;

    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [Header("Rotation")] [SerializeField] private RotationAxis axis = RotationAxis.Z;
    [SerializeField] private bool invert = false;
    [SerializeField] private int symbolsPerReel = 11;

    [Header("Spin Tuning")] [SerializeField]
    private float spinDuration = 3.0f; // общий момент финиша

    [SerializeField] private float stopStagger = 0.35f; // старт по очереди, но финиш общий
    [SerializeField] private Vector2 extraTurnsRange = new Vector2(3, 6); // мин..макс целых оборотов
    [SerializeField] private AnimationCurve decelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // (1,1)!

    [Header("Segments (non-uniform)")] [SerializeField]
    private float angleZeroOffsetDeg = 0f; // общий сдвиг «верха»

    [SerializeField] private float[] segmentMidAnglesDeg; // длиной = symbolsPerReel

    // локальные состояния
    private Quaternion[] _baseRot; // базовая локальная ориентация каждого рила
    private float[] _reelAngle; // наш «накопительный» угол каждого рила (0..360)
    public bool spinning;
    private readonly int[] _lastTargets = new int[3];

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
        if (!IsServer)
        {
            ServerSpinStart();
            return;
        }

        // если мы сервер/хост — сразу запускаем
        ServerSpinStart();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerSpinStart()
    {
        if (!PlayerCharacter.Instance.PlayerWallet.TryBuy(_price))
            return;
        if (spinning) return;
        spinning = true;
        WalletOperation(_price, false);
        _netWorkAnimator.PlayAnim("Spin");

        int[] targets = new int[3];
        GenerateTargetsWithChances(targets, symbolsPerReel, 0.10f, 0.25f);

        int[] extras = new int[3];
        int minT = Mathf.Max(0, Mathf.CeilToInt(extraTurnsRange.x));
        int maxT = Mathf.Max(minT, Mathf.CeilToInt(extraTurnsRange.y));
        for (int i = 0; i < 3; i++)
            extras[i] = Random.Range(minT, maxT + 1);

        RpcSpinAll(targets, extras, spinDuration, stopStagger);
    }

    // roll с шансами: pTriple и pDouble — доли 0..1 (например 0.05 и 0.25)
    private void GenerateTargetsWithChances(int[] targets, int symbolCount, float pTriple, float pDouble)
    {
        if (targets == null || targets.Length < 3) return;
        if (symbolCount < 2)
        {
            // с 1 символом все равно будет три одинаковых
            targets[0] = targets[1] = targets[2] = 0;
            return;
        }

        float roll = Random.value;

        if (roll < pTriple)
        {
            // 5% — три одинаковых
            int idx = Random.Range(0, symbolCount);
            targets[0] = targets[1] = targets[2] = idx;
        }
        else if (roll < pTriple + pDouble)
        {
            // 25% — ровно две одинаковых
            int baseIdx = Random.Range(0, symbolCount);
            // выбираем пару барабанов, которые совпадут: (0,1), (0,2) или (1,2)
            int pair = Random.Range(0, 3);
            int a = (pair == 0) ? 0 : (pair == 1) ? 0 : 1;
            int b = (pair == 0) ? 1 : (pair == 1) ? 2 : 2;

            targets[a] = baseIdx;
            targets[b] = baseIdx;

            // третий — любой другой, не равный baseIdx
            int c = 3 - a - b; // индекс оставшегося барабана
            targets[c] = NextDifferentIndex(baseIdx, symbolCount);
        }
        else
        {
            // остальное — рандом (может случайно дать 2 или 3 одинаковых, если так хочется исключить — добавь проверку)
            targets[0] = Random.Range(0, symbolCount);
            targets[1] = Random.Range(0, symbolCount);
            targets[2] = Random.Range(0, symbolCount);
        }
    }

    private int NextDifferentIndex(int notThis, int symbolCount)
    {
        int idx = Random.Range(0, symbolCount - 1);
        // «дырка» на notThis: сдвигаем всё >= notThis на +1
        return (idx >= notThis) ? idx + 1 : idx;
    }

    [ObserversRpc(BufferLast = false)]
    private void RpcSpinAll(int[] targetIdx, int[] extraTurns, float duration, float stagger)
    {
        if (_audio) _audio.Play();

        for (int i = 0; i < reels.Length; i++)
        {
            float delay = i * stagger; // по очереди
            float durI = duration; // все крутятся одинаковое время
            StartCoroutine(SpinSingleRoutine(i, targetIdx[i], extraTurns[i], durI, delay));
        }

        Array.Copy(targetIdx, _lastTargets, 3);

        // ждать окончания ПОСЛЕДНЕГО барабана
        float total = duration + (reels.Length - 1) * stagger + 0.05f;
        StartCoroutine(ClearAndCheckAfter(total));
    }

    private IEnumerator ClearAndCheckAfter(float t)
    {
        yield return new WaitForSeconds(t);
        spinning = false;
        if (IsServer)
        {
            CheckResults();
        }
    }

    private IEnumerator ClearSpinningAfter(float t)
    {
        yield return new WaitForSeconds(t);
        spinning = false;
    }

    private IEnumerator SpinSingleRoutine(int reelIndex, int targetIndex, int extraTurnsInt, float duration,
        float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        _audioBaraban.UnPause();

        Transform reel = reels[reelIndex];
        Vector3 axisVec = (axis == RotationAxis.X) ? Vector3.right :
            (axis == RotationAxis.Y) ? Vector3.up : Vector3.forward;
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
        float endAngle = startAngle + totalTravel; // конечный абсолютный угол
        bool preStopPlayed = false;
        const float preStopWindow = 0.2f;

        // плавное торможение до endAngle — без пост-снапа
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float eased = decelCurve.Evaluate(Mathf.Clamp01(t / duration)); // последний ключ (1,1)!
            float cur = Mathf.Lerp(startAngle, endAngle, eased); // линейный по абсолютному углу
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

    private void CheckResults()
    {
        int a = _lastTargets[0];
        int b = _lastTargets[1];
        int c = _lastTargets[2];

        if (a == b && b == c)
        {
            Debug.Log($"BIG WIN!! [{a} {b} {c}]  (symbol {a})");
            WalletOperation(_priceBigWin, true);
            return;
        }

        if (a == b || a == c || b == c)
        {
            int matchedSymbol, oddSymbol;
            string pair;
            if (a == b)
            {
                matchedSymbol = a;
                oddSymbol = c;
                pair = "0&1";
            }
            else if (a == c)
            {
                matchedSymbol = a;
                oddSymbol = b;
                pair = "0&2";
            }
            else
            {
                matchedSymbol = b;
                oddSymbol = a;
                pair = "1&2";
            }

            Debug.Log($"Small win: pair {pair}, symbol {matchedSymbol}, odd {oddSymbol}. [{a} {b} {c}]");
            WalletOperation(_priceWin, true);
            return;
        }

        // НИЧЕГО
        Debug.Log($"No win [{a} {b} {c}]");
    }

    [ObserversRpc]
    private void WalletOperation(int money, bool addmoney)
    {
        if (addmoney)
        {
            PlayerCharacter.Instance.PlayerWallet.Add(money);
        }
        else
        {
            PlayerCharacter.Instance.PlayerWallet.Spend(money);
        }
    }
}