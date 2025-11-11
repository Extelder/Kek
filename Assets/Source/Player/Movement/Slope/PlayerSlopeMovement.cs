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
    [SerializeField] private float _maxSlopeAngle;

    private RaycastHit _hit;

    private PlayerCharacter _character;
    
    private CompositeDisposable _reactiveDisposable = new CompositeDisposable();
    private CompositeDisposable _checkDisposable = new CompositeDisposable();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            return;
        _character = PlayerCharacter.Instance;
        _playerMovement.Moving.Subscribe(_ =>
        {
            if (_)
            {
                CheckNormalAngle();
                return;
            }
            _checkDisposable.Clear();
        }).AddTo(_reactiveDisposable);
    }

    private void CheckNormalAngle()
    {
        Observable.Interval(TimeSpan.FromSeconds(_checkRate)).Subscribe(_ =>
        {
            if (OnSlope())
            {
                _character.Rigidbody.AddForce(GetSlopeMoveDirection() * _playerMovement.Speed * 20f, ForceMode.Force);

                if (_character.Rigidbody.velocity.y > 0)
                    _character.Rigidbody.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }).AddTo(_checkDisposable);
    }
    
    private bool OnSlope()
    {
        bool hitted = Physics.Raycast(_raycastSettings.Origin.position, Vector3.down, out _hit,
            _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
        if (hitted)
        {
            if (_hit.collider.TryGetComponent<Ground>(out Ground ground))
            {
                float angle = Vector3.Angle(Vector3.up, _hit.normal);
                return angle < _maxSlopeAngle && angle != 0;
            }
        }

        return false;
    }
    
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_playerMovement.InputVector, _hit.normal).normalized;
    }

    private void OnDisable()
    {
        if (!base.IsOwner)
            return;
        _checkDisposable.Clear();
        _reactiveDisposable.Clear();
    }
}
