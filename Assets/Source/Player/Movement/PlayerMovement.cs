using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UniRx;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private PlayerCharacter _character;
    
    [SerializeField] private float _speed;
    
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _decceleration;

    private Rigidbody _rigidbody;
    private PlayerBinds _binds;

    private CompositeDisposable _disposable = new CompositeDisposable();


    private Vector3 _currentVelocity;

    public bool Moving { get; private set; }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!base.IsOwner)
            return;
        
        _rigidbody = _character.Rigidbody;
        _binds = _character.Binds;

        Vector3 inputVector = new Vector3(0, 0, 0);

        Observable.EveryUpdate().Subscribe(_ =>
        {
            inputVector = new Vector3(_binds.Character.Horizontal.ReadValue<float>(), 0,
                _binds.Character.Vertical.ReadValue<float>());
        }).AddTo(_disposable);

        Observable.EveryFixedUpdate().Subscribe(_ =>
        {
            if(!IsOwner)
                return;
            inputVector = transform.TransformDirection(inputVector);

            inputVector.Normalize();

            Moving = Mathf.Abs(inputVector.x) > 0 || Mathf.Abs(inputVector.z) > 0;

            Vector3 desiredVelocityXZ = new Vector3(inputVector.x * _speed, 0,
                inputVector.z * _speed);

            if (Moving == true || _groundChecker.Detected == true)
                _currentVelocity =
                    Vector3.MoveTowards(_currentVelocity, desiredVelocityXZ, _acceleration * Time.fixedDeltaTime);
            else if (Moving == false)
            {
                _currentVelocity =
                    Vector3.MoveTowards(_currentVelocity, desiredVelocityXZ, _decceleration * Time.fixedDeltaTime);
            }

            _rigidbody.velocity =
                new Vector3(_currentVelocity.x, _rigidbody.velocity.y, _currentVelocity.z);
        }).AddTo(_disposable);
    }

  
    private void OnDisable()
    {
        _disposable?.Clear();
    }
}
