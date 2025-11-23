using System;
using FishNet.Object;
using UniRx;
using UnityEngine;

public class PlayerSlopeMovement : NetworkBehaviour
{
    [Header("Raycast")]
    [SerializeField] private RaycastSettings _raycastSettings;
    [SerializeField] private float _checkRate = 0.05f;
    [SerializeField] private float _maxSlopeAngle = 45f;

    [Header("Refs")]
    [SerializeField] private PlayerMovement _playerMovement;

    private PlayerCharacter _character;

    private RaycastHit _hit;
    private readonly CompositeDisposable _moveDisposable = new CompositeDisposable();
    private readonly CompositeDisposable _checkDisposable = new CompositeDisposable();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
            return;

        _character = PlayerCharacter.Instance;

        // слушаем начало/конец движения
        _playerMovement.Moving
            .Subscribe(isMoving =>
            {
                if (isMoving)
                    StartSlopeCheck();
                else
                    _checkDisposable.Clear();
            })
            .AddTo(_moveDisposable);
    }

    private void StartSlopeCheck()
    {
        _checkDisposable.Clear();

        Observable.Interval(TimeSpan.FromSeconds(_checkRate))
            .Subscribe(_ =>
            {
                if (IsOnSlope())
                    ApplySlopeMovement();
            })
            .AddTo(_checkDisposable);
    }

    private bool IsOnSlope()
    {
        bool hit = Physics.Raycast(
            _raycastSettings.Origin.position,
            Vector3.down,
            out _hit,
            _raycastSettings.MaxDistance,
            _raycastSettings.LayerMask);

        if (!hit)
            return false;

        if (!_hit.collider.TryGetComponent<Ground>(out _))
            return false;

        float angle = Vector3.Angle(_hit.normal, Vector3.up);
        return angle > 0 && angle <= _maxSlopeAngle;
    }

    private void ApplySlopeMovement()
    {
        Vector3 dir = GetSlopeMoveDirection();
        Rigidbody rb = _character.Rigidbody;

        // мягкое "прилипание" к склону
        if (rb.velocity.y > 0)
            rb.AddForce(Vector3.down * 40f, ForceMode.Acceleration);

        // движение вдоль склона
        rb.AddForce(
            dir * _playerMovement.Speed * 10f,
            ForceMode.Acceleration);
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_playerMovement.InputVector, _hit.normal).normalized;
    }

    private void OnDisable()
    {
        if (!IsOwner)
            return;

        _moveDisposable.Clear();
        _checkDisposable.Clear();
    }
}
