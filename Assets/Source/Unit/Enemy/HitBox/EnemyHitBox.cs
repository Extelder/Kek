using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.Mathematics;
using UnityEngine;

public class EnemyHitBox : NetworkBehaviour, IWeaponVisitor
{
    [SerializeField] private EnemyStateMachine _enemyStateMachine;
    [SerializeField] private EnemyHealth _enemyHealth;

    public void Visit(RPGProjectile rpgProjectile)
    {
        Hit(rpgProjectile.Damage, transform.position + new Vector3(0, 0.5f, 0));
    }

    public void Visit(TNTThrowable tntThrowable)
    {
        _enemyStateMachine?.Kite(tntThrowable.Transform);
        Hit(tntThrowable.Damage, transform.position);
    }

    public void Visit(Pickaxe pickaxe, RaycastHit hit)
    {
        HitWithRaycast(pickaxe.Damage, hit.point, hit.normal);
    }

    public void Visit(Drill drill, RaycastHit hit)
    {
        HitWithRaycast(drill.Damage, hit.point, hit.normal);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HitWithRaycast(float damage, Vector3 point, Vector3 normal)
    {
        HitWithRaycastObsrever(damage, point, normal);
    }

    [ObserversRpc]
    public void HitWithRaycastObsrever(float damage, Vector3 point, Vector3 normal)
    {
        _enemyHealth.TakeDamage(damage);
        Pools.Instance.BloodPool.GetFreeElement(point, Quaternion.LookRotation(normal));
    }

    [ServerRpc(RequireOwnership = false)]
    public void Hit(float damage, Vector3 bloodPoint)
    {
        HitObsrever(damage, bloodPoint);
    }

    [ObserversRpc]
    public void HitObsrever(float damage, Vector3 bloodPoint)
    {
        _enemyHealth.TakeDamage(damage);
        Pools.Instance.BloodPool.GetFreeElement(bloodPoint, quaternion.identity);
    }
}