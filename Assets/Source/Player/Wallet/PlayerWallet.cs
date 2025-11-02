using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerWallet : NetworkBehaviour
{
    [SerializeField] private int _startValue = 0;
    [SerializeField] private int _minValue = 0;

    public bool AlredySpend { get; set; }

    [field: SerializeField] public int CurrentValue { get; private set; }

    private const int maxValue = Int32.MaxValue;

    public event Action<int> ValueChanged;
    public event Action<int> MoneyChanged;

    public override void OnStartClient()
    {
        base.OnStartClient();
        AddSeparately(_startValue);
    }
    

    [ServerRpc(RequireOwnership = false)]
    public void Spend(int value)
    {
        SpendObserver(value);
    }

    [ObserversRpc]
    private void SpendObserver(int value)
    {
        if (CurrentValue - value < _minValue)
        {
            AlredySpend = false;
            return;
        }

        CurrentValue -= value;
        AlredySpend = true;
        MoneyChanged?.Invoke(-value);
        ValueChanged?.Invoke(CurrentValue);
    }

    [ServerRpc(RequireOwnership = false)]
    public void Add(int value)
    {
        AddObserver(value);
    }

    [ObserversRpc]
    private void AddObserver(int value)
    {
        if (CurrentValue + value > maxValue)
        {
            CurrentValue = maxValue;
            ValueChanged?.Invoke(CurrentValue);
            return;
        }

        CurrentValue += value;
        MoneyChanged?.Invoke(value);
        ValueChanged?.Invoke(CurrentValue);
    }

    public void AddSeparately(int value)
    {
        AddObserver(value);
    }
}
