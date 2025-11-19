using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMeshMove : NetworkBehaviour
{
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }

    [ServerRpc(RequireOwnership = false)]
    public void SetDestinationServer(Vector3 position)
    {
        SetDestinationObserver(position);
    }

    [ObserversRpc]
    public void SetDestinationObserver(Vector3 position)
    {
        if (position != null)
            Agent?.SetDestination(position);
    }
}