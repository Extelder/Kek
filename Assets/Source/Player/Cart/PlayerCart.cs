using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using UniRx;
using UnityEngine;

public class PlayerCart : NetworkBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _maxMoveDelta = 0.3f;
    
    [SerializeField] private Transform[] _wheels;
    [SerializeField] private float _wheelRadius = 0.2f;
    [SerializeField] private float _deadZone = 0.05f;
    [SerializeField] private float _smoothLerp = 12f;
    private Vector3 _lastPos;
    private bool _hasLast;
    private float _smoothedForwardSpeed;
    private bool _equiped;

    private PlayerCharacter _currentPlayer;

    public void Interact(PlayerCharacter character)
    {
        InteractServer(character);
    }

    [ServerRpc(RequireOwnership = false)]
    public void InteractServer(PlayerCharacter character)
    {
        InteractObserver(character);
    }

    private void Update()
    {
        float forwardSpeed = 0f;
        if (_hasLast)
        {
            Vector3 delta = transform.position - _lastPos;
            Vector3 vel = delta / Time.deltaTime;
            forwardSpeed = Vector3.Dot(vel, transform.forward);
        }
        _lastPos = transform.position;
        _hasLast = true;
        if (Mathf.Abs(forwardSpeed) < _deadZone) forwardSpeed = 0f;
        _smoothedForwardSpeed = (_smoothLerp > 0f)
            ? Mathf.Lerp(_smoothedForwardSpeed, forwardSpeed, 1f - Mathf.Exp(-_smoothLerp * Time.deltaTime))
            : forwardSpeed;
        float radius = Mathf.Max(_wheelRadius, 0.0001f);
        float degPerFrame = (_smoothedForwardSpeed / radius) * Mathf.Rad2Deg * Time.deltaTime;
        for (int i = 0; i < _wheels.Length; i++)
        {
            var w = _wheels[i];
            w.Rotate(Vector3.forward, degPerFrame, Space.Self);
        }
    }

    [ObserversRpc]
    public void InteractObserver(PlayerCharacter character)
    {
        if (character != _currentPlayer)
        {
            if (_equiped)
            {
                return;
            }

            _equiped = true;
            _currentPlayer = character;
        }
        else
        {
            if (_equiped)
            {
                _equiped = false;
                return;
            }

            return;
        }
    }

    private void FixedUpdate()
    {
        if (!_equiped)
            return;
        FollowPlayer();
    }


    private void FollowPlayer()
    {
        var target = _currentPlayer.CartPoint;

        Vector3 targetPos = target.position;
        Vector3 toTarget = targetPos - _rigidbody.position;

        if (toTarget.magnitude > _maxMoveDelta)
            toTarget = toTarget.normalized * _maxMoveDelta;

        Vector3 newPos = _rigidbody.position + toTarget * _moveSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(newPos);

        Quaternion targetRot = target.rotation;
        Quaternion newRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, _rotateSpeed * Time.fixedDeltaTime);
        _rigidbody.MoveRotation(newRot);
    }
}