using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Quota : NetworkBehaviour
{
    [SerializeField] private float _removeValue;

    [SerializeField] private Generator _generator;
    [SerializeField] private float _minValuetoHire;
    [SerializeField] private float _startValue;

    public float CurrentValue { get; private set; }

    public event Action<float> ValueChanged;

    public override void OnStartClient()
    {
        Debug.Log(base.IsServer + " Server");

        if (!base.IsServer)
        {
            Debug.LogError("Sex");
            return;
        }

        _generator.GenerateStarted += OnGenerateStarted;
        CurrentValue += _startValue;
        ValueChanged?.Invoke(CurrentValue);
    }

    private void OnGenerateStarted()
    {
        SpendServer(_removeValue);
    }

    private void OnDisable()
    {
        if (!base.IsServer)
            return;

        _generator.GenerateStarted -= OnGenerateStarted;
    }

    public bool TryBuy(float value)
    {
        return (CurrentValue - value >= _minValuetoHire);
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
        CurrentValue += value;
        ValueChanged?.Invoke(CurrentValue);
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

    public void Spend(float value)
    {
        if (CurrentValue - value < _minValuetoHire)
        {
            return;
        }

        CurrentValue -= value;
        Debug.Log(CurrentValue);
        ValueChanged?.Invoke(CurrentValue);
    }
}