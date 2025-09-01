using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : NetworkBehaviour
{
    [SerializeField] private PlayerCharacter _character;
    [SerializeField] private float _jumpForce;
    [SerializeField] private GroundChecker _groundChecker;

    private Rigidbody _rigidbody;
    private PlayerBinds _binds;

    private bool _jumping;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!base.IsOwner)
            return;
        _binds = _character.Binds;
        _rigidbody = _character.Rigidbody;

        _binds.Character.Jump.started += JumpKeyDowned;
    }

    private void OnDisable()
    {
        if(!base.IsOwner)
            return;
        StopAllCoroutines();
        _binds.Character.Jump.started -= JumpKeyDowned;
    }
    private void JumpKeyDowned(InputAction.CallbackContext obj)
    {
        if (_groundChecker.Detected)
            Jump();
    }

    private void Jump()
    {
        _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
}