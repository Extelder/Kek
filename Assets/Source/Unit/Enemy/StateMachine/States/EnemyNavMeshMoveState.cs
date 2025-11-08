using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMeshMoveState : EnemyState
{
    [SerializeField] private float _updateTargetMovePositionRate;
    [SerializeField] private NavMeshAgent _agent;

    private Transform _playerTransform;

    private void Start()
    {
        _playerTransform = PlayerCharacter.Instance.PlayerTransform;
    }

    public override void Enter()
    {
        EnemyAnimator.Move();
        StartCoroutine(Moving());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Moving()
    {
        while (true)
        {
            yield return new WaitForSeconds(_updateTargetMovePositionRate);
            _agent.SetDestination(_playerTransform.position);
        }
    }
}
