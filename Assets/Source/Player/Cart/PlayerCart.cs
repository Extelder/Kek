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