using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Utilities;
using Observable = UniRx.Observable;
using Random = UnityEngine.Random;

public class EnemyRandomMove : NetworkBehaviour
{
    [SerializeField] private EnemyNavMeshMove _enemyNavMeshMove;
    [SerializeField] private Vector3 _center;

    [SerializeField] private float _radius;
    [SerializeField] private float _minimalRemainingDistance;

    private Vector3 _result;

    public override void OnStartServer()
    {
        base.OnStartClient();
        StartCoroutine(GettingRandomPointOnNavMesh());
    }

    public bool GetRandomPointOnNavMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _radius;
        Vector3 randomPoint = transform.position + randomDirection;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, _radius, NavMesh.AllAreas))
        {
            _result = hit.position;

            return true;
        }

        _result = Vector3.zero;
        return false;
    }

    private bool ReachedPoint() => _enemyNavMeshMove.Agent.remainingDistance <= _minimalRemainingDistance;

    private IEnumerator GettingRandomPointOnNavMesh()
    {
        while (true)
        {
            yield return new WaitUntil(() => GetRandomPointOnNavMesh());
            _enemyNavMeshMove.SetDestinationServer(_result);
            yield return new WaitUntil(() => ReachedPoint());
        }
    }

    private void OnDisable()
    {
        if (!base.IsOwner)
            return;
        StopAllCoroutines();
    }
}