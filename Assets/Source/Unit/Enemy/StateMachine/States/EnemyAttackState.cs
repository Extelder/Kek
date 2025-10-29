using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    [field: SerializeField] public EnemyDamage Damage { get; private set; }

    public PlayerHitBox PlayerHitBox { get; private set; }

    public override void Enter()
    {
        Animator.Attack();
    }

    public void PerformAttack()
    {
        PlayerHitBox.TakeDamage(Damage.GetDamage());
    }

    public virtual void OnPlayerDetected(PlayerHitBox hitBox)
    {
        PlayerHitBox = hitBox;
    }

    protected virtual void OnDisable()
    {
        
    }
}