using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private PlayerDie die;
    
    public override void Death()
    {
        CurrentValue = 0;
        HealthValueChanged?.Invoke(CurrentValue);

        die.Die();
        Debug.Log("Пизда");
    }
}