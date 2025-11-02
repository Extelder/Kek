using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerWallet : NetworkBehaviour
{
    [SerializeField] private int _minValue;
    [SerializeField] private int _startValue;

    public int CurrentValue { get; private set; }

    public event Action<int> ValueChanged;

    public override void OnStartClient()
    {
        if (!base.IsOwner)
            return;
        CurrentValue += _startValue;
        ValueChanged?.Invoke(CurrentValue);
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpendServer(1);
        }
    }

    private void Add(int value)
    {
        CurrentValue += value;
        ValueChanged?.Invoke(CurrentValue);
    }

    public void SpendBithc(int value)
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

    private void Spend(int value)
    {
        CurrentValue -= value;
        ValueChanged?.Invoke(CurrentValue);
    }
}