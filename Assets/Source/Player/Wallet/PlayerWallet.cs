using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerWallet : NetworkBehaviour
{
    [SerializeField] private int _minValue;
    [SerializeField] private int _startValue;

    private int _maxValue = Int32.MaxValue;
    public int CurrentValue { get; private set; }

    public event Action<int> ValueChanged;
    public event Action<int> MoneyChanged;

    public override void OnStartClient()
    {
        if (!base.IsOwner)
            return;
        CurrentValue += _startValue;
        ValueChanged?.Invoke(CurrentValue);
    }

    public bool TryBuy(int value)
    {
        return (CurrentValue - value >= _minValue);
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
        MoneyChanged?.Invoke(+value);
    }

    public void Add(int value)
    {
        if (CurrentValue + value > _maxValue)
        {
            CurrentValue = _maxValue;
            MoneyChanged?.Invoke(+value);
            ValueChanged?.Invoke(CurrentValue);
            return;
        }

        CurrentValue += value;
        ValueChanged?.Invoke(CurrentValue);
        MoneyChanged?.Invoke(+value);
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
        if (CurrentValue - value < _minValue)
        {
            return;
        }

        CurrentValue -= value;
        ValueChanged?.Invoke(CurrentValue);
        MoneyChanged?.Invoke(-value);
    }
}