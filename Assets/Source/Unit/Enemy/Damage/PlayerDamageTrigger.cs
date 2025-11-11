using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageTrigger : PlayerTrigger
{
    [SerializeField] private EnemyDamage _damage;
    public override void OnTriggered(PlayerHealth playerHealth)
    {
        playerHealth.TakeDamage(_damage.GetDamage());
    }
}
