using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public abstract class Stamina : NetworkBehaviour
{
    [field: SerializeField] public float MaxValue { get; set; }
    protected float CurrentValue { get; set; } = 100;

    public Action<float> StaminaValueChanged;
    public event Action<float> OnAddedToMax;
    public event Action<float> Spended;
    public event Action<float> Added;
    public event Action IsLost;

    private void Start()
    {
        VirtualStart();
    }

    public virtual void VirtualStart()
    {
        CurrentValue = MaxValue;
    }

    public virtual void Spend(float value)
    {
        if (IsSpend())
            return;

        if (CurrentValue - value > 0)
        {
            ChangeStaminaValue(CurrentValue - value);
            Spended?.Invoke(CurrentValue);
            return;
        }

        CurrentValue = 0;
        Spended?.Invoke(CurrentValue);

        IsLost?.Invoke();
        Lost();
    }

    public void Add(float value)
    {
        if (CurrentValue + value < MaxValue)
        {
            Added?.Invoke(CurrentValue);
            ChangeStaminaValue(CurrentValue + value);
            return;
        }

        SetToMax();
        Added?.Invoke(CurrentValue);
    }

    public bool IsSpend() => CurrentValue <= 0;

    public void SetToMax()
    {
        if (IsSpend())
            return;
        ChangeStaminaValue(MaxValue);
        OnAddedToMax?.Invoke(MaxValue);
    }

    public abstract void Lost();

    protected virtual void ChangeStaminaValue(float value)
    {
        if (CurrentValue >= 0)
        {
            CurrentValue = value;
            StaminaValueChanged?.Invoke(CurrentValue);
        }
    }

    public float GetCurrentValue() => CurrentValue;

    public void SetCurrentValue(float value)
    {
        CurrentValue = value;
        if (value > MaxValue)
            CurrentValue = MaxValue;
        if (value <= 0)
            CurrentValue = 0;
        StaminaValueChanged?.Invoke(CurrentValue);
    }
}
