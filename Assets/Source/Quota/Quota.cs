using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UnityEngine;

public class Quota : NetworkBehaviour
{
    [SerializeField] private float _removeValue;

    [SerializeField] private Generator _generator;
    [SerializeField] public float _minValuetoHire;
    [SerializeField] private float _startValue;
    [SerializeField] private float _currentValue;

    public event Action<float> ValueChanged;

    public override void OnStartServer()
    {
        _generator.GenerateStarted += OnGenerateStarted;
        _currentValue += _startValue;
        ValueChanged?.Invoke(_currentValue);
        NotifyValueChanged(_currentValue);
    }

    private void OnGenerateStarted()
    {
        Spend(_removeValue);
    }
    private void OnDisable()
    {
        if (!base.IsServer)
            return;

        _generator.GenerateStarted -= OnGenerateStarted;
    }

    [ObserversRpc(BufferLast = true)]
    private void NotifyValueChanged(float newValue)
    {
        _currentValue = newValue;
        ValueChanged?.Invoke(newValue);
    }

    public bool TryBuy(float value)
    {
        return (_currentValue - value  >= _minValuetoHire);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddServer(float value)
    {
        AddObserever(value);
    }

    [ObserversRpc]
    public void AddObserever(float value)
    {
        Add(value);
    }

    public void Add(float value)
    {
        _currentValue += value;
        ValueChanged?.Invoke(_currentValue);
    }

    public void SpendMoney(float value)
    {
        SpendServer(value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpendServer(float value)
    {
        SpendObserever(value);
    }

    [ObserversRpc]
    public void SpendObserever(float value)
    {
        Spend(value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void Spend(float value)
    {
        if (!IsServer)
            return;

        _currentValue -= value;
        NotifyValueChanged(_currentValue);
    }
}