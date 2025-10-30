using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyState
{
    [SerializeField] private NavMeshAgent _agent;
    [field: SerializeField] public EnemyDamage Damage { get; private set; }

    public PlayerHitBox PlayerHitBox { get; private set; }

    public event Action AttackAnimationEnded;

    public override void Enter()
    {
        if (!base.IsServer)
            return;
        CanChanged = false;
        Animator.Attack();
        _agent.isStopped = true;
    }

    public void PerformAttack()
    {
        if (!base.IsServer)
            return;
        PlayerHitBox.TakeDamage(Damage.GetDamage());
    }

    public virtual void OnPlayerDetected(PlayerHitBox hitBox)
    {
        if (!base.IsServer)
            return;
        PlayerHitBox = hitBox;
    }

    public void AttackAnimationEnd()
    {
        if (!base.IsServer)
            return;
        _agent.isStopped = false;
        CanChanged = true;
        AttackAnimationEnded?.Invoke();
    }
}