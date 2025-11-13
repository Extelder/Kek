using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
public class MoveProjectileOnEnable : MonoBehaviour
{
    [SerializeField] private Transform _projectile;
    [SerializeField] private float _maxDistance;
    
    private CompositeDisposable _disposable = new CompositeDisposable();
    private void OnEnable()
    {
        Move();
    }

    private void Move()
    {
        _disposable.Clear();
        Observable.EveryUpdate().Subscribe(_ =>
        {
            _projectile.position = Vector3.MoveTowards(_projectile.position, _projectile.forward, _maxDistance);
        }).AddTo(_disposable);
    }

    private void OnDisable()
    {
        _disposable.Clear();
    }
}
