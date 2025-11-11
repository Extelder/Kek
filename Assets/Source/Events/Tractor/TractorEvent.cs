using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

public class TractorEvent : RandomEvent
{
    private PlayerCharacter _target;
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }

    public override void StartEvent()
    {
        StartEventServer();
        _target = FindNearestPlayerCharacter(transform.position);
        StartEventServer();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartEventServer()
    {
        SetDestinationObserver();
    }

    [ObserversRpc]
    private void SetDestinationObserver()
    {
        _target = FindNearestPlayerCharacter(transform.position);
        Agent.SetDestination(_target.transform.position);
    }

    private void Update()
    {
        Agent.SetDestination(_target.transform.position);
    }

    private PlayerCharacter FindNearestPlayerCharacter(Vector3 fromPosition)
    {
        PlayerCharacter[] characters = PlayerCharacter.Instance.Characters.ToArray();
        PlayerCharacter nearest = null;
        float minDistSq = float.MaxValue;

        foreach (var character in characters)
        {
            if (character == null) continue;

            float distSq = (character.transform.position - fromPosition).sqrMagnitude;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                nearest = character;
            }
        }

        return nearest;
    }
}