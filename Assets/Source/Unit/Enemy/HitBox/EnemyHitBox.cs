using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EnemyHitBox : NetworkBehaviour ,IWeaponVisitor
{
    [SerializeField] private EnemyStateMachine _enemyStateMachine;
    [SerializeField] private EnemyHealth _enemyHealth;
    public void Visit(RPGProjectile rpgProjectile)
    {
        Hit(rpgProjectile.Damage);
    }

    public void Visit(TNTThrowable tntThrowable)
    {
        _enemyStateMachine.Kite(tntThrowable.Transform);
        Hit(tntThrowable.Damage);
    }

    public void Visit(Pickaxe pickaxe, RaycastHit hit)
    {
        Hit(pickaxe.Damage);
    }

    public void Visit(Drill drill, RaycastHit hit)
    {
        Hit(drill.Damage);
    }

    [ServerRpc(RequireOwnership = false)]
    public void Hit(float damage)
    {
        HitObsrever(damage);
    }

    [ObserversRpc]
    public void HitObsrever(float damage)
    {
        _enemyHealth.TakeDamage(damage);   
    }
}
