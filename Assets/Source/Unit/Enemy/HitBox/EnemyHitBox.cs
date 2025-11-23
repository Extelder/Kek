using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.Mathematics;
using UnityEngine;

public class EnemyHitBox : UnitHitBox
{
    [SerializeField] private EnemyStateMachine _enemyStateMachine;

    public override void Visit(TNTThrowable tntThrowable)
    {
        base.Visit(tntThrowable);
        _enemyStateMachine?.Kite(tntThrowable.Transform);
    }
}