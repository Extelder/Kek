using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKiteState : EnemyState
{
    [SerializeField] private EnemyStateMachine _enemyStateMachine;
    [SerializeField] private EnemyNavMeshMove _enemyNavMeshMove;
    
    [SerializeField] private float _coolDown;
    [SerializeField] private float _updateTargetRate;
    [SerializeField] private float _distanceToStop;

    private Transform _target;
    public override void Enter()
    {
        if (!base.IsServer)
            return;
        CanChanged = false;
        StopAllCoroutines();
        StartCoroutine(SetTargetDestination());
    }

    private IEnumerator Kiting()
    {
        EnemyAnimator.Kait();
        yield return new WaitForSeconds(_coolDown);
        CanChanged = true;
        _enemyStateMachine.Patrol();
    }

    private IEnumerator SetTargetDestination()
    {
        while (true)
        {
            if (_enemyNavMeshMove.Agent.remainingDistance <= _distanceToStop)
                StartCoroutine(Kiting());
            EnemyAnimator.Run();
            _enemyNavMeshMove.SetDestinationServer(_target.position);
            yield return new WaitForSeconds(_updateTargetRate);
        }
    }

    public void ChangeTarget(Transform tnt)
    {
        _target = tnt;
    }
}
