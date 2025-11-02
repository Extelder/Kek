using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UniRx;
using UnityEngine;

public class PlayerEyesLookAt : NetworkBehaviour
{
    [SerializeField] private OverlapSettings _overlapSettings;
    [SerializeField] private RaycastSettings _raycastSettings;
    [SerializeField] private int _maxColliderCount;
    [SerializeField] private float _checkRate;
    [SerializeField] private Collider _collider;

    [SerializeField] private List<PlayerCharacter> _characters;
    [SerializeField] private Collider[] _playerColliders;

    private CompositeDisposable _disposable = new CompositeDisposable();

    public override void OnStartClient()
    {
        base.OnStartClient();

        _playerColliders = new Collider[_maxColliderCount];
        Debug.Log("CollidersCapacity" + _playerColliders.Length);
        Observable.Interval(TimeSpan.FromSeconds(_checkRate)).Subscribe(_ =>
        {
            Debug.Log("NOTHING");
            Overlap();
        }).AddTo(_disposable);
    }

    private void Overlap()
    {
        Debug.Log("overlap");
        _overlapSettings.Size = Physics.OverlapSphereNonAlloc(_overlapSettings.Origin.position,
            _overlapSettings.SphereRadius, _playerColliders, _overlapSettings.LayerMask);
        foreach (var other in _playerColliders)
        {
            if (other == null || other == _collider)
            {
                continue;
            }

            if (other.TryGetComponent<PlayerCharacter>(out PlayerCharacter playerCharacter))
            {
                SearchNearestEnemy(playerCharacter);
            }
        }
    }

    public void SearchNearestEnemy(PlayerCharacter playerCharacter)
    {
        _characters.Clear();
        if (Physics.Raycast(_raycastSettings.Origin.position,
            (playerCharacter.TargetPoint.position - _raycastSettings.Origin.position),
            out RaycastHit hit, _raycastSettings.MaxDistance, _raycastSettings.LayerMask))
        {
            if (hit.collider == _collider)
            {
                return;
            }
            if (hit.collider.TryGetComponent<PlayerCharacter>(
                out PlayerCharacter PlayerCharacter))
            {
                Debug.Log("PlayerRaycast");
                PlayerCharacter.Distance = hit.distance;
                _characters.Add(playerCharacter);
            }
        }

        PlayerCharacter currentPlayerCharacter = null;
        float minDistance = Single.PositiveInfinity;
        foreach (var other in _characters)
        {
            if (other.Distance < minDistance)
            {
                currentPlayerCharacter = other;
                minDistance = other.Distance;
            }
        }

        LookAtPlayer(currentPlayerCharacter);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_overlapSettings.Origin.position, _overlapSettings.SphereRadius);
    }

    private void LookAtPlayer(PlayerCharacter playerCharacter)
    {
        Debug.Log("Player Look At" + playerCharacter);
        if (playerCharacter == null)
            return;
        transform.LookAt(playerCharacter.TargetPoint, transform.forward);
    }

    private void OnDisable()
    {
        _disposable.Clear();
    }
}