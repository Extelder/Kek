using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    [SerializeField] private State _patrolState;
    [SerializeField] private State _attack;
    [SerializeField] private EnemyChaseState _chase;

    public void Patrol()
    {
        if (!base.IsServer)
            return;
        ChangeState(_patrolState);
    }

    public void Attack()
    {
        if (!base.IsServer)
            return;
        ChangeState(_attack);
    }

    public void Chase(Transform player)
    {
        if (!base.IsServer)
            return;
        _chase.ChangeTarget(player);
        ChangeState(_chase);
    }
}