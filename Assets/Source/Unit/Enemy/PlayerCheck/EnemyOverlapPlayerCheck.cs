using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EnemyOverlapPlayerCheck : EnemyPlayerCheck
{
    [SerializeField] private EnemyAttackState _enemyAttackState;

    [field: SerializeField] public EnemyStateMachine StateMachine { get; private set; }
    [SerializeField] private OverlapSettings _overlappSettings;
    [SerializeField] private float _checkRate;
    [SerializeField] private int _colliderCount;

    private Collider[] _others;

    private CompositeDisposable _disposable = new CompositeDisposable();

    public override event Action<PlayerHitBox> PlayerDetected;
    public override event Action PlayerLost;

    public override void OnStartClient()
    {
        if (!base.IsServer)
            return;
        StartChecking();
    }

    public override void StartChecking()
    {
        Observable.Interval(TimeSpan.FromSeconds(_checkRate)).Subscribe(_ =>
        {
            _others = new Collider[_colliderCount];
            Physics.OverlapSphereNonAlloc(_overlappSettings.Origin.position, _overlappSettings.SphereRadius, _others,
                _overlappSettings.LayerMask);

            for (int i = 0; i < _others.Length; i++)
            {
                if (_others[i] == null)
                {
                    continue;
                }

                if (_others[i].TryGetComponent<PlayerHitBox>(out PlayerHitBox playerHitBox))
                {
                    _enemyAttackState.OnPlayerDetected(playerHitBox);
                    PlayerDetected?.Invoke(playerHitBox);
                    OnPlayerDetected();
                    return;
                }
            }


            _enemyAttackState.OnPlayerDetected(null);
            PlayerLost?.Invoke();
        }).AddTo(_disposable);
    }

    public virtual void OnPlayerDetected()
    {
        StateMachine.Attack();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_overlappSettings.Origin.position, _overlappSettings.SphereRadius);
    }

    public override void StopChecking()
    {
        _disposable?.Clear();
    }

    private void OnDisable()
    {
        if (!base.IsServer)
            return;
        StopChecking();
    }
}