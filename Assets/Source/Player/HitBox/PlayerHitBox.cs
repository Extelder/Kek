using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerHitBox : NetworkBehaviour
{
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private float _notActiveDelayAfterSpawn;

    private bool _active;

    public override void OnStartClient()
    {
        StopAllCoroutines();
        StartCoroutine(WaitForDelay());
    }


    private IEnumerator WaitForDelay()
    {
        yield return new WaitForSeconds(_notActiveDelayAfterSpawn);
        _active = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDamage(float damage)
    {
        if (!_active)
            return;
        TakeDamageObserver(damage, _health);
    }

    [ObserversRpc]
    public void TakeDamageObserver(float damage, PlayerHealth health)
    {
        health.TakeDamage(damage);
    }
}