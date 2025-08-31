using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _character;
    [SerializeField] private float _jumpForce;
    [SerializeField] private GroundChecker _groundChecker;

    private Rigidbody _rigidbody;
    private PlayerBinds _binds;

    private bool _jumping;

    private void Start()
    {
        _binds = _character.Binds;
        _rigidbody = _character.Rigidbody;

        _binds.Character.Jump.started += JumpKeyDowned;
    }

    private void OnDisable()
    {
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