using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyState
{
    [SerializeField] private EnemyAttackState _enemyAttackState;

    [SerializeField] private EnemyNavMeshMove _enemyNavMeshMove;
    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private float _updateTargetRate;
    public Transform Target { get; private set; }

    public void ChangeTarget(Transform target)
    {
        if (!base.IsServer)
            return;
        Target = target;
        _enemyAttackState.AttackAnimationEnded += OnAttackAnimationEnded;
    }

    private void OnAttackAnimationEnded()
    {
        Enter();
    }

    public override void Enter()
    {
        if (!base.IsServer)
            return;
        StopAllCoroutines();
        StartCoroutine(Chasing());
    }

    public override void Exit()
    {
        if (!base.IsServer)
            return;
        Animator.Idle();
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        if (!base.IsServer)
            return;
        _enemyAttackState.AttackAnimationEnded += OnAttackAnimationEnded;

        StopAllCoroutines();
    }

    private IEnumerator Chasing()
    {
        while (true)
        {
            Animator.Run();
            _enemyNavMeshMove.SetDestinationServer(Target.position);
            yield return new WaitForSeconds(_updateTargetRate);
        }
    }
}