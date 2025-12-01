using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float Decceleration { get; set; }
    public Vector3 InputVector { get; private set; } = new Vector3(0, 0, 0);

    [SerializeField] private PlayerAnimator _animator;

    [SerializeField] private PlayerCharacter _character;

    [SerializeField] private float _runSpeedMultiplier;

    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private float _acceleration;

    private Rigidbody _rigidbody;
    private PlayerBinds _binds;

    private CompositeDisposable _disposable = new CompositeDisposable();

    private Vector3 _currentVelocity;

    private float _speedMultiplier = 1;

    public bool CanFly;
    public float Invert = 1;

    public bool CanRun = true;

    public ReactiveProperty<bool> Moving { get; private set; } = new ReactiveProperty<bool>();

    private bool _running;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            return;

        _rigidbody = _character.Rigidbody;
        _binds = _character.Binds;

        Observable.EveryUpdate().Subscribe(_ =>
        {
            InputVector = new Vector3(_binds.Character.Horizontal.ReadValue<float>(), 0,
                _binds.Character.Vertical.ReadValue<float>());
        }).AddTo(_disposable);

        _binds.Character.Run.started += OnRunStarted;
        _binds.Character.Run.canceled += OnRunCanceled;

        Observable.EveryFixedUpdate().Subscribe(_ =>
        {
            if (!IsOwner)
                return;
            InputVector = transform.TransformDirection(InputVector);

            InputVector.Normalize();

            Moving.Value = Mathf.Abs(InputVector.x) > 0 || Mathf.Abs(InputVector.z) > 0;


            Vector3 desiredVelocityXZ = new Vector3(InputVector.x * Speed * _speedMultiplier * Invert, 0,
                InputVector.z * Speed * _speedMultiplier * Invert);

            if (Moving.Value || _groundChecker.Detected)
                _currentVelocity =
                    Vector3.MoveTowards(_currentVelocity, desiredVelocityXZ, _acceleration * Time.fixedDeltaTime);
            else if (!Moving.Value)
            {
                _currentVelocity =
                    Vector3.MoveTowards(_currentVelocity, desiredVelocityXZ, Decceleration * Time.fixedDeltaTime);
            }

            if (CanFly)
            {
                float fly = _binds.Character.FlyUpDown.ReadValue<float>();
                _rigidbody.velocity =
                    new Vector3(_currentVelocity.x, fly * Speed, _currentVelocity.z);
                return;
            }

            _rigidbody.velocity =
                new Vector3(_currentVelocity.x, _rigidbody.velocity.y, _currentVelocity.z);
        }).AddTo(_disposable);
    }

    private void OnRunCanceled(InputAction.CallbackContext obj)
    {
        if (!CanRun)
            return;
        _running = false;
        Speed /= _runSpeedMultiplier;
        _animator.SetLocomotionBlendTreeSpeed(1f);
    }

    private void OnRunStarted(InputAction.CallbackContext obj)
    {
        if (!CanRun)
            return;
        _running = true;
        Speed *= _runSpeedMultiplier;
        _animator.SetLocomotionBlendTreeSpeed(1.5f);
    }

    private void Update()
    {
        Debug.LogError(Speed);
    }


    public void StopRun()
    {
        if (_running)
        {
            Speed /= _runSpeedMultiplier;
            _animator.SetLocomotionBlendTreeSpeed(1f);

            _running = false;
        }
    }


    public void BoostSpeed(float value, float forTime)
    {
        StopAllCoroutines();
        _speedMultiplier = value;
        StartCoroutine(WaitForeBoost(forTime));
    }

    private IEnumerator WaitForeBoost(float time)
    {
        yield return new WaitForSeconds(time);
        _speedMultiplier = 1;
    }

    private void OnDisable()
    {
        if (!base.IsOwner)
            return;

        _binds.Character.Run.started += OnRunStarted;
        _binds.Character.Run.canceled += OnRunCanceled;

        _disposable?.Clear();
    }
}