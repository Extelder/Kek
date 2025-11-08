using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyState
{
    [SerializeField] private EnemyNavMeshMove _enemyNavMeshMove;
    [SerializeField] private float _randomPointRange;
    [SerializeField] private float _maxRandomWaitTime;
    [SerializeField] private NavMeshAgent _agent;

    public override void Enter()
    {
        if (!base.IsServer)
            return;
        StopAllCoroutines();
        StartCoroutine(Patroling());
    }

    public override void Exit()
    {
        if (!base.IsServer)
            return;
        StopAllCoroutines();
    }

    private IEnumerator Patroling()
    {
        while (true)
        {
            if (GetRandomPointOnNavMesh(transform.position, _randomPointRange, out Vector3 point))
            {
                EnemyAnimator.Move();
                _enemyNavMeshMove.SetDestinationServer(point);
            }

            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => AgentReachedDestination());
            EnemyAnimator.Idle();
            yield return new WaitForSeconds(Random.Range(0, _maxRandomWaitTime));
        }
    }

    public bool GetRandomPointOnNavMesh(Vector3 center, float radius, out Vector3 result)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        Vector3 randomPoint = center + randomDirection;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, radius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private bool AgentReachedDestination() => _agent.remainingDistance <= 0.2f;
}