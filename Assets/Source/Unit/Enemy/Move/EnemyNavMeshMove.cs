using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMeshMove : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    [ServerRpc(RequireOwnership = false)]
    public void SetDestinationServer(Vector3 position)
    {
        SetDestinationObserver(position);
    }


    [ObserversRpc]
    public void SetDestinationObserver(Vector3 position)
    {
        _agent.SetDestination(position);
    }
}