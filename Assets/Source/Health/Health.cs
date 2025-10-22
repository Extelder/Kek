using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    [field: SerializeField] public float MaxValue { get; set; }
    protected float CurrentValue { get; set; }

    public event Action<float> HealthValueChanged;
    public event Action<float> OnHealedToMax;
    public event Action<float> Damaged;
    public event Action<float> Healed;
    public event Action Dead;

    private void Start()
    {
        VirtualStart();
    }

    public virtual void VirtualStart()
    {
        CurrentValue = MaxValue;
    }

    public virtual void TakeDamage(float value)
    {
        if (IsDead())
            return;

        if (CurrentValue - value > 0)
        {
            ChangeHealthValue(CurrentValue - value);
            Damaged?.Invoke(CurrentValue);
            return;
        }

        Damaged?.Invoke(CurrentValue);

        Dead?.Invoke();
        Death();
    }

    public void Heal(float value)
    {
        if (IsDead())
            return;
        if (CurrentValue + value < MaxValue)
        {
            Healed?.Invoke(CurrentValue);
            ChangeHealthValue(CurrentValue + value);
            return;
        }

        HealToMax();
        Healed?.Invoke(CurrentValue);
    }

    public bool IsDead() => CurrentValue <= 0;

    public void HealToMax()
    {
        if (IsDead())
            return;
        ChangeHealthValue(MaxValue);
        OnHealedToMax?.Invoke(MaxValue);
    }

    public void Armor(float addibleHealth)
    {
        MaxValue *= addibleHealth;
        CurrentValue = MaxValue;
    }

    public abstract void Death();

    protected virtual void ChangeHealthValue(float value)
    {
        if (CurrentValue > 0)
        {
            CurrentValue = value;
            HealthValueChanged?.Invoke(CurrentValue);
        }
    }

    public void SetCurrentValue(float value)
    {
        CurrentValue = value;
        if (value > MaxValue)
            CurrentValue = MaxValue;
        if (value <= 0)
            CurrentValue = 0;
        HealthValueChanged?.Invoke(CurrentValue);
    }
}