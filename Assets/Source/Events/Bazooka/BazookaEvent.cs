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
    [SerializeField] private float _delayUpdate;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private float _delayAfterAttack;
    [SerializeField] private GameObject _objectToOff;
    [SerializeField] private GameObject _objectForSpawn;
    [SerializeField] private GameObject _objectToSpawn;
    [SerializeField] private float _deathTimer;
    [SerializeField] private float _chanceIsDrop;
    [SerializeField] private SoundEventBazooka _soundEventBazooka;
    private bool _attack;

    public override void StartEvent()
    {
        StartEventServer();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartEventServer()
    {
        _target = FindNearestPlayerCharacter(transform.position);
        if (Agent != null && _target != null)
            Agent.SetDestination(_target.transform.position);
        StartCoroutine(UpdateWithDelay());
        StartCoroutine(DeathByTimer());
        StartSound();
    }
    [ObserversRpc]
    private void StartSound()
    {
        _soundEventBazooka.StartCoroutine(_soundEventBazooka.FootStep());
    }

    [ObserversRpc]
    private void OffObject()
    {
        _objectToOff.SetActive(false);
    }

    private IEnumerator DeathByTimer()
    {
        yield return new WaitForSeconds(_deathTimer);
        OffObject();
        float chance = Random.Range(1, 101);
        if (chance >= _chanceIsDrop)
            PlayerCharacter.Instance.ServerSpawnObject(_objectForSpawn, _objectToSpawn.transform.position,_objectToSpawn.transform.rotation);
        yield return new WaitForSeconds(3f);
        PlayerCharacter.Instance.Despawn(gameObject);
    }

    private PlayerCharacter FindNearestPlayerCharacter(Vector3 fromPosition)
    {
        if (PlayerCharacter.Instance == null)
            return null;
        PlayerCharacter[]
            characters = PlayerCharacter.Instance.Characters.ToArray();
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
            Agent.SetDestination(_target.transform.position);
            AttackWait();
        }
    }

    private void AttackWait()
    {
        if (_attack)
        {
            return;
        }

        if (Physics.Raycast(transform.position, _target.transform.position - transform.position, out RaycastHit hit,
            10000f, _mask))
        {
            if (!hit.collider.GetComponent<PlayerCharacter>())
            {
                return;
            }
        }

        _attack = true;
        if (_audio) _audio.Play();
        Attack();
        StartCoroutine(WaitAfterAttack());
    }

    private IEnumerator WaitAfterAttack()
    {
        yield return new WaitForSeconds(_delayAfterAttack);
        _attack = false;
    }

    private void Attack()
    {
        _bazooka.Attack(_target);
    }
}