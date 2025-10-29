using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachine
{
    [SerializeField] private State _attack;
    [SerializeField] private State _chase;
    
    public void Attack()
    {
        ChangeState(_attack);
    }

    public void Chase(Transform player)
    {
        ChangeState(_chase);
    }
}
