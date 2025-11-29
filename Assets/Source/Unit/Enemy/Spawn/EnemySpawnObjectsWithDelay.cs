using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UniRx;
using UnityEngine;

public class EnemySpawnObjectsWithDelay : NetworkBehaviour
{
    [SerializeField] private RaycastSettings _raycastSettings;
    [SerializeField] private GameObject _spawnableGameObject;
    [SerializeField] private float _spawnCooldown;

    private CompositeDisposable _disposable = new CompositeDisposable();

    public override void OnStartServer()
    {
        base.OnStartServer();
        Spawn();
    }

    private void Spawn()
    {
        Observable.Interval(TimeSpan.FromSeconds(_spawnCooldown)).Subscribe(_ =>
        {
            bool hitted = Physics.Raycast(_raycastSettings.Origin.position, Vector3.down, out RaycastHit hit,
                _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
            if (hitted)
            {
                PlayerCharacter.Instance.ServerSpawnObject(_spawnableGameObject, hit.point,
                    Quaternion.FromToRotation(transform.up, hit.normal));
            }
        }).AddTo(_disposable);
    }

    private void OnDisable()
    {
        _disposable.Clear();
    }
}