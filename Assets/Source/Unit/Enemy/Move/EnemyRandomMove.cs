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

    public override void OnStartServer()
    {
        base.OnStartClient();
        StartCoroutine(GettingRandomPointOnNavMesh());
    }

    public bool GetRandomPointOnNavMesh(out Vector3 result)
    {
        Debug.Log("Getting");
        Vector3 randomDirection = Random.insideUnitSphere * _radius;
        Vector3 randomPoint = _center + randomDirection;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, _radius, NavMesh.AllAreas))
        {
            result = hit.position;
            Debug.Log("SetDestination");
            if (_enemyNavMeshMove.Agent.remainingDistance <= _minimalRemainingDistance)
                _enemyNavMeshMove.SetDestinationServer(result);

            return true;
        }

        result = Vector3.zero;
        return GetRandomPointOnNavMesh(out result);
    }

    private IEnumerator GettingRandomPointOnNavMesh()
    {
        yield return new WaitUntil(()=> GetRandomPointOnNavMesh(out Vector3 result));
    }
}
