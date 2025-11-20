using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.Mathematics;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private NetworkObject _object;

    private bool _dead;

    public override void Death()
    {
        if (_dead)
            return;
        _dead = true;
        OnDestroYServer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnDestroYServer()
    {
        OnDestroYObserver();
    }

    [ObserversRpc]
    public void OnDestroYObserver()
    {
        Pools.Instance.BloodExplodePool.GetFreeElement(transform.position + new Vector3(0, 1f, 0), Quaternion.identity,
            () =>
            {
                if (_object != null) _object.Despawn();
            });
    }
}