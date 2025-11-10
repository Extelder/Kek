using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using Observable = UniRx.Observable;

public class PlayerSlopeMovement : NetworkBehaviour
{
    [SerializeField] private RaycastSettings _raycastSettings;

    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private float _checkRate;
    
    private CompositeDisposable _reactiveDisposable = new CompositeDisposable();
    private CompositeDisposable _checkDisposable = new CompositeDisposable();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            return;
        _playerMovement.Moving.Subscribe(_ =>
        {
            if (_)
                CheckNormalAngle();
        }).AddTo(_reactiveDisposable);
    }

    private void CheckNormalAngle()
    {
        Observable.Interval(TimeSpan.FromSeconds(_checkRate)).Subscribe(_ =>
        {
            bool hitted = Physics.Raycast(_raycastSettings.Origin.position, Vector3.down, out RaycastHit hit,
                _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
            if (hitted)
            {
                if (hit.collider.TryGetComponent<Ground>(out Ground ground))
                {
                }
            }
        }).AddTo(_checkDisposable);
    }

    private void OnDisable()
    {
        if (!base.IsOwner)
            return;
        _checkDisposable.Clear();
        _reactiveDisposable.Clear();
    }
}
