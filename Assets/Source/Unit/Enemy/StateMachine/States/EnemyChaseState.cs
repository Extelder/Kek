using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyState
{
    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private float _updateTargetRate;
    public Transform Target { get; private set; }

    public void ChangeTarget(Transform target)
    {
        Target = target;
    }

    public override void Enter()
    {
        StopAllCoroutines();
        StartCoroutine(Chasing());
    }

    public override void Exit()
    {
        Animator.Idle();
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Chasing()
    {
        while (true)
        {
            Animator.Run();
            _agent.SetDestination(Target.position);
            yield return new WaitForSeconds(_updateTargetRate);
        }
    }
}