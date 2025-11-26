using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class TriggerTrap : PlayerTrigger
{
    [SerializeField] private int _damage;

    public override void OnTriggered(PlayerHealth playerHealth)
    {
        playerHealth.TakeDamage(_damage);
    }
}