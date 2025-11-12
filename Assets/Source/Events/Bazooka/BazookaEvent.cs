using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

public class BazookaEvent : RandomEvent
{
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [SerializeField] private AudioSource _audio;
    [SerializeField] private BazookaAttack _bazooka;
    private PlayerCharacter _target;
    private float _delayUpdate;
    [SerializeField] private LayerMask _mask;
    private float _delayAfterAttack;
    private bool _attack;

    public override void StartEvent()
    {
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
        if (Agent != null && _target != null)
            Agent.SetDestination(_target.transform.position);
        if (_audio) _audio.Play();
        StartCoroutine(UpdateWithDelay());
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

    private IEnumerator UpdateWithDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(_delayUpdate);
            _target = FindNearestPlayerCharacter(transform.position);
            if (Agent != null && _target != null)
                Agent.SetDestination(_target.transform.position);
            
        }
    }

    private IEnumerator AttackWait()
    {
        if (_attack)
        {
            yield break;
        }
        _attack = true;
        Attack(_target);
        yield return new WaitForSeconds(_delayAfterAttack);
        _attack = false;
    }

    private void Attack(PlayerCharacter AttackLocation)
    {
        _bazooka.Attack(AttackLocation);
    }
}