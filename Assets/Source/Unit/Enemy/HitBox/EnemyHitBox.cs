using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour ,IWeaponVisitor
{
    [SerializeField] private EnemyStateMachine _enemyStateMachine;
    public void Visit(RPGProjectile rpgProjectile)
    {
    }

    public void Visit(TNTThrowable tntThrowable)
    {
        _enemyStateMachine.Kite(tntThrowable.Transform);
    }

    public void Visit(Pickaxe pickaxe, RaycastHit hit)
    {
    }

    public void Visit(Drill drill, RaycastHit hit)
    {
    }
}
