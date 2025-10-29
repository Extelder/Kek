using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyState
{
    [SerializeField] private NavMeshAgent _agent;
    [field: SerializeField] public EnemyDamage Damage { get; private set; }

    public PlayerHitBox PlayerHitBox { get; private set; }

    public override void Enter()
    {
        CanChanged = false;
        Animator.Attack();
        _agent.isStopped = true;
    }

    public void PerformAttack()
    {
        PlayerHitBox.TakeDamage(Damage.GetDamage());
    }

    public virtual void OnPlayerDetected(PlayerHitBox hitBox)
    {
        PlayerHitBox = hitBox;
    }

    public void AttackAnimationEnd()
    {
        _agent.isStopped = false;
        CanChanged = true;
    }
}