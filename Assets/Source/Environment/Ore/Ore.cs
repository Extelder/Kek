using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Ore : NetworkBehaviour, IWeaponVisitor
{
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private Transform _modelsOrigin;
    [SerializeField] private float _scaleDifference;
    [SerializeField] private float _scaleThresholdToDestroy;

    public event Action Destroyed;

    public void Visit(RPGProjectile rpgProjectile)
    {
        DestroyOre();
    }

    public void Visit(TNTThrowable tntThrowable)
    {
    }

    public void Visit(Pickaxe pickaxe, RaycastHit hit)
    {
        PlayerCharacter.Instance.ServerSpawnObject(_hitEffect, hit.point, Quaternion.LookRotation(-hit.normal));
        Hit();
    }

    public void Visit(Drill drill, RaycastHit hit)
    {
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
        Despawn();
    }
}