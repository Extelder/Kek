using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    [SerializeField] private State _patrolState;
    [SerializeField] private State _attack;
    [SerializeField] private EnemyKiteState _kite;
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

    public void Kite(Transform tnt)
    {
        if (!base.IsServer)
            return;
        _kite.ChangeTarget(tnt);
        ChangeState(_kite);
    }

    public void Chase(Transform player)
    {
        if (!base.IsServer)
            return;
        _chase.ChangeTarget(player);
        ChangeState(_chase);
    }
}