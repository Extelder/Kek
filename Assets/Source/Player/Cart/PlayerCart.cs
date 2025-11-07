using System;
using FishNet.Object;
using UnityEngine;

public class PlayerCart : NetworkBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _rotateSpeed = 12f;
    [SerializeField] private float _maxMoveDelta = 0.4f;
    [SerializeField] private float _heightAdjustSpeed = 8f;

    [SerializeField] private Transform[] _wheels;
    [SerializeField] private float _wheelRadius = 0.2f;
    [SerializeField] private float _smoothLerp = 12f;

    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundCheckDistance = 1f;

    private PlayerCharacter _currentPlayer;
    private bool _equipped;
    private Vector3 _lastPos;
    private float _smoothedForwardSpeed;
    private Vector3 _velocity;
    private Vector3 _targetVel;

    public void Interact(PlayerCharacter character)
    {
        InteractServer(character);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServer(PlayerCharacter character)
    {
        InteractObserver(character);
    }

    [ObserversRpc]
    private void InteractObserver(PlayerCharacter character)
    {
        if (_currentPlayer != character)
        {
            _currentPlayer = character;
            _equipped = true;
            _rigidbody.useGravity = false;
        }
        else
        {
            _equipped = !_equipped;
            _rigidbody.useGravity = !_equipped;
            if (!_equipped)
                _currentPlayer = null;
        }
    }

    private void FixedUpdate()
    {
        if (!_equipped || _currentPlayer == null)
            return;

        FollowPlayerSmooth();
        AnimateWheels();
    }

    private void FollowPlayerSmooth()
    {
        Transform target = _currentPlayer.CartPoint;

        Vector3 desiredPos = target.position;
        Vector3 toTarget = desiredPos - _rigidbody.position;

        if (toTarget.magnitude > _maxMoveDelta)
            toTarget = toTarget.normalized * _maxMoveDelta;

        _targetVel = toTarget * _moveSpeed;
        _velocity = Vector3.Lerp(_velocity, _targetVel, 1f - Mathf.Exp(-_smoothLerp * Time.fixedDeltaTime));
        _rigidbody.velocity = _velocity;

        Quaternion targetRot = target.rotation;
        Quaternion newRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, _rotateSpeed * Time.fixedDeltaTime);
        _rigidbody.MoveRotation(newRot);

        if (Physics.Raycast(_rigidbody.position + Vector3.up * 0.3f, Vector3.down, out RaycastHit hit, _groundCheckDistance, _groundMask))
        {
            Vector3 groundNormal = hit.normal;
            Vector3 adjustedPos = new Vector3(_rigidbody.position.x, hit.point.y, _rigidbody.position.z);
            _rigidbody.position = Vector3.Lerp(_rigidbody.position, adjustedPos, _heightAdjustSpeed * Time.fixedDeltaTime);

            Quaternion groundTilt = Quaternion.FromToRotation(_rigidbody.transform.up, groundNormal) * _rigidbody.rotation;
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, groundTilt, 0.5f * Time.fixedDeltaTime));
        }
    }

    private void AnimateWheels()
    {
        Vector3 delta = _rigidbody.position - _lastPos;
        float forwardSpeed = Vector3.Dot(delta / Time.fixedDeltaTime, transform.forward);
        _lastPos = _rigidbody.position;

        _smoothedForwardSpeed = Mathf.Lerp(_smoothedForwardSpeed, forwardSpeed, 1f - Mathf.Exp(-_smoothLerp * Time.fixedDeltaTime));
        float radius = Mathf.Max(_wheelRadius, 0.0001f);
        float degPerFrame = (_smoothedForwardSpeed / radius) * Mathf.Rad2Deg * Time.fixedDeltaTime;

        foreach (var w in _wheels)
            w.Rotate(Vector3.forward, degPerFrame, Space.Self);
    }
}
