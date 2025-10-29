using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    [SerializeField] private State _patrolState;
    [SerializeField] private State _attack;
    [SerializeField] private EnemyChaseState _chase;

    public void Attack()
    {
        ChangeState(_attack);
    }

    public void Chase(Transform player)
    {
        _chase.ChangeTarget(player);
        ChangeState(_chase);
    }
}