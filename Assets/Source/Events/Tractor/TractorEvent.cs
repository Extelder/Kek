using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class TractorEvent : RandomEvent
{
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [SerializeField] private AudioSource _audio;
    private PlayerCharacter _target;
    [SerializeField] Transform[] wheels;
    [SerializeField] private GameObject _tractorGood;
    [SerializeField] float rpm = 180f;

    public enum Axis
    {
        PlusX,
        MinusX,
        PlusY,
        MinusY,
        PlusZ,
        MinusZ
    }

    [SerializeField] Axis spinAxis = Axis.PlusX;

    Vector3 AxisVec => spinAxis switch
    {
        Axis.PlusX => Vector3.right, Axis.MinusX => Vector3.left,
        Axis.PlusY => Vector3.up, Axis.MinusY => Vector3.down,
        Axis.PlusZ => Vector3.forward, Axis.MinusZ => Vector3.back
    };

    public override void StartEvent()
    {
        StartEventServer();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartEventServer()
    {
        SetDestinationObserver();
        WaitAndDestroy();
        _target = FindNearestPlayerCharacter(transform.position);
        if (Agent != null && _target != null)
            Agent.SetDestination(_target.transform.position);
    }

    [ObserversRpc]
    private void SetDestinationObserver()
    {
        if (_audio) _audio.Play();
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

    private IEnumerator WaitAndDestroy()
    {

        float random = Random.Range(2,3);    
        yield return new WaitForSeconds(random);
        Death();
    }
    
    private void Death()
    {
        PlayerCharacter.Instance.ServerSpawnObject(_tractorGood, transform.position, transform.rotation);
        Despawn(gameObject);
    }

    private void Update()
    {
        if (!IsServer) return;
        Agent.SetDestination(_target.transform.position);
        float deg = rpm * 6f * Time.deltaTime; // 360/60
        var ax = AxisVec;
        for (int i = 0; i < (wheels?.Length ?? 0); i++)
            if (wheels[i])
                wheels[i].Rotate(ax, deg, Space.Self);
    }
}