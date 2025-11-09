using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Quota : NetworkBehaviour
{
    [SerializeField] private float _minValuetoHire;
    [SerializeField] private float _startValue;
    
    public float CurrentValue { get; private set; }

    public event Action<float> ValueChanged;

    public override void OnStartClient()
    {
        if (!base.IsOwner)
            return;
        CurrentValue += _startValue;
        ValueChanged?.Invoke(CurrentValue);
    }

    public bool TryBuy(int value)
    {
        return (CurrentValue - value >= _minValuetoHire);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddServer(int value)
    {
        AddObserever(value);
    }

    [ObserversRpc]
    public void AddObserever(int value)
    {
        Add(value);
    }

    public void Add(int value)
    {
        CurrentValue += value;
        ValueChanged?.Invoke(CurrentValue);
    }

    public void SpendMoney(int value)
    {
        SpendServer(value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpendServer(int value)
    {
        SpendObserever(value);
    }

    [ObserversRpc]
    public void SpendObserever(int value)
    {
        Spend(value);
    }

    public void Spend(int value)
    {
        if (CurrentValue - value < _minValuetoHire)
        {
            return;
        }

        CurrentValue -= value;
        ValueChanged?.Invoke(CurrentValue);
    }
}