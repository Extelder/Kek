using System;
using System.Collections;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class TransportHAB : NetworkBehaviour
{
    [SerializeField] private Transform _objectToTransform;

    [Header("9 состояний (точки в мире)")] [SerializeField]
    private Transform[] _states = new Transform[9];

    [Header("Параметры движения")] [SerializeField]
    private float _moveTime = 1f;

    [SerializeField] private AnimationCurve _moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private int _currentStateIndex; // 0..8
    [SerializeField] private LocationChanger _locationChanger;
    [SerializeField] private TextMeshPro _textDrillig;
    [SerializeField] private TextMeshPro _textDrilligTochki;
    [SerializeField] private TextMeshPro _depth;
    private Coroutine _moveRoutine;
    [SerializeField] private Renderer _renderer;
    private int stage = 1;
    private int depth = 0;

    private void OnEnable()
    {
        _locationChanger.StartRegenerate += OnExternalEvent;
    }

    private void OnDisable()
    {
        _locationChanger.StartRegenerate -= OnExternalEvent;
    }

    public int CurrentStateIndex
    {
        get => _currentStateIndex;
        set => _currentStateIndex = Mathf.Clamp(value, 0, _states.Length - 1);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnExternalEvent()
    {
        _currentStateIndex++;
        if (_currentStateIndex >= 8)
        {
            _currentStateIndex = 0;
            stage++;
            UpdateStageColor();
        }

        MoveToState(_currentStateIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetStateAndMove(int index)
    {
        CurrentStateIndex = index;
        MoveToState(CurrentStateIndex);
    }

    [ObserversRpc]
    private void MoveToState(int index)
    {
        if (_objectToTransform == null ||
            _states == null ||
            index < 0 || index >= _states.Length ||
            _states[index] == null)
        {
            Debug.LogWarning("TransportHAB: точки состояний настроены криво");
            return;
        }

        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = StartCoroutine(MoveRoutine(_states[index]));
    }

    private IEnumerator MoveRoutine(Transform target)
    {
        _textDrillig.enabled = true;
        _textDrilligTochki.enabled = true;
        StartCoroutine(MetrDepth());
        StartCoroutine(Tochki());
        Vector3 startPos = _objectToTransform.position;
        Vector3 endPos = target.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _moveTime;
            float k = _moveCurve.Evaluate(Mathf.Clamp01(t));

            _objectToTransform.position = Vector3.Lerp(startPos, endPos, k);
            yield return null;
        }

        _moveRoutine = null;
        _textDrillig.enabled = false;
        _textDrilligTochki.enabled = false;
    }

    private IEnumerator MetrDepth()
    {
        while (_textDrillig.enabled)
        {
            yield return new WaitForSeconds(0.5f);
            int randomDepth = UnityEngine.Random.Range(2, 4);
            depth = randomDepth + depth;
            _depth.text = depth.ToString();
        }
    }

    private IEnumerator Tochki()
    {
        int currentTochki = 1;
        while (_textDrillig.enabled)
        {
            yield return new WaitForSeconds(0.2f);
            _textDrilligTochki.text = new string('.', currentTochki);
            currentTochki++;
            if (currentTochki == 4)
            {
                currentTochki = 1;
            }
        }
    }

    public void UpdateStageColor()
    {
        float t = Mathf.Clamp01(stage / 10f);

        Color targetColor = Color.Lerp(Color.white, Color.red, t);

        _renderer.material.color = targetColor;
    }
}