using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Ore : HitBox
{
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private Transform _modelsOrigin;
    [SerializeField] private float _scaleDifference;
    [SerializeField] private float _scaleThresholdToDestroy;

    public event Action Destroyed;

    public override void Visit(RPGProjectile rpgProjectile)
    {
        base.Visit(rpgProjectile);
        DestroyOre();
    }

    public override void Visit(TNTThrowable tntThrowable)
    {
        base.Visit(tntThrowable);
    }

    public override void Visit(Pickaxe pickaxe, RaycastHit hit)
    {
        base.Visit(pickaxe, hit);
        PlayerCharacter.Instance.ServerSpawnObject(_hitEffect, hit.point, Quaternion.LookRotation(-hit.normal));
        Hit();
    }

    public override void Visit(Drill drill, RaycastHit hit)
    {
        base.Visit(drill, hit);
        PlayerCharacter.Instance.ServerSpawnObject(_hitEffect, hit.point, Quaternion.LookRotation(-hit.normal));
        Hit();
    }

    [ServerRpc(RequireOwnership = false)]
    public void Hit()
    {
        HitObsrever();
    }

    [ObserversRpc]
    public void HitObsrever()
    {
        _modelsOrigin.localScale -= new Vector3(_scaleDifference, _scaleDifference, _scaleDifference);

        if (_modelsOrigin.localScale.x <= _scaleThresholdToDestroy ||
            _modelsOrigin.localScale.y <= _scaleThresholdToDestroy ||
            _modelsOrigin.localScale.z <= _scaleThresholdToDestroy)
        {
            DestroyOre();
        }
    }

    private void DestroyOre()
    {
        Destroyed?.Invoke();
        StartCoroutine(WaitForDespawn());
    }

    private IEnumerator WaitForDespawn()
    {
        yield return new WaitForSeconds(0.5f);
        Despawn();
    }
}