using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyState
{
    [SerializeField] private EnemyChaseState _chaseState;
    [SerializeField] private bool _stopNavMesh = true;
    [SerializeField] private NavMeshAgent _agent;
    [field: SerializeField] public EnemyDamage Damage { get; private set; }

    public PlayerHitBox PlayerHitBox { get; private set; }

    public event Action AttackAnimationEnded;

    public override void Enter()
    {
        if (!base.IsServer)
            return;
        CanChanged = false;
        EnemyAnimator.Attack();
        _agent.isStopped = _stopNavMesh;
        if (!_agent.isStopped)
        {
            StartCoroutine(_chaseState.ChasingWithoutAnimation());
        }
    }

    public override void Exit()
    {
        _chaseState.StopAllCoroutines();
    }

    public void PerformAttack()
    {
        if (!base.IsServer)
            return;
        PlayerHitBox?.TakeDamage(Damage.GetDamage());
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
        if (_stopNavMesh)
            _agent.isStopped = false;
        CanChanged = true;
        AttackAnimationEnded?.Invoke();
    }
}