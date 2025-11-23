using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyState
{
    [SerializeField] private EnemyStateMachine _enemyStateMachine;

    [SerializeField] private float _lostTime;

    [SerializeField] private EnemyAttackState _enemyAttackState;

    [SerializeField] private EnemyNavMeshMove _enemyNavMeshMove;

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
        StartCoroutine(WaitingTimeForLost());
    }

    public override void Exit()
    {
        if (!base.IsServer)
            return;
        EnemyAnimator.Idle();
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        if (!base.IsServer)
            return;
        _enemyAttackState.AttackAnimationEnded -= OnAttackAnimationEnded;

        StopAllCoroutines();
    }

    private IEnumerator WaitingTimeForLost()
    {
        yield return new WaitForSeconds(_lostTime);
        _enemyStateMachine.Patrol();
    }

    private IEnumerator Chasing()
    {
        while (true)
        {
            OnStartedChasing();
            CallAnimations();
            if (Target != null)
                _enemyNavMeshMove.SetDestinationServer(Target.position);
            yield return new WaitForSeconds(_updateTargetRate);
        }
    }

    public virtual void OnStartedChasing()
    {
    }

    public virtual void CallAnimations()
    {
        EnemyAnimator.Run();
    }

    public IEnumerator ChasingWithoutAnimation()
    {
        while (true)
        {
            _enemyNavMeshMove.SetDestinationServer(Target.position);
            yield return new WaitForSeconds(_updateTargetRate);
        }
    }
}