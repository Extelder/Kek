using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FishNet.Object;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

public class WormEvent : RandomEvent
{
    [SerializeField] private CinemachineImpulseSource _cinemachineImpulse;

    [SerializeField] private float _retargetDelay;
    [SerializeField] private float _lookAtSpeed;
    [SerializeField] private float _moveSpeed;

    [SerializeField] private float _waitForTired;

    private PlayerCharacter _target;

    [SerializeField] private Collider _mountAndBlade;

    private CompositeDisposable _disposable = new CompositeDisposable();

    private bool _tired = false;

    public override void StartEvent()
    {
        _target =
            PlayerCharacter.Instance.Characters[Random.Range(0, PlayerCharacter.Instance.Characters.Count)];

        _mountAndBlade.OnCollisionEnterAsObservable().Subscribe(_ =>
            {
                ShakeServer();
                if (_tired)
                    return;
                if (_.gameObject.TryGetComponent<PlayerHitBox>(out PlayerHitBox PlayerHitBox))
                {
                    Debug.LogError("Player");

                    StopAllCoroutines();
                    StartCoroutine(WaitingForNewPlayer());
                }
            }
        ).AddTo(_disposable);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShakeServer()
    {
        ShakeObserver();
    }


    [ObserversRpc]
    private void ShakeObserver()
    {
        _cinemachineImpulse.GenerateImpulse();
    }

    private IEnumerator WaitingForNewPlayer()
    {
        yield return new WaitForSeconds(_waitForTired);
        _target = null;

        _tired = true;

        yield return new WaitForSeconds(_retargetDelay);

        _tired = false;

        _target =
            PlayerCharacter.Instance.Characters[Random.Range(0, PlayerCharacter.Instance.Characters.Count)];
    }

    private void OnDisable()
    {
        if (!base.IsServer)
            return;
        _disposable.Clear();
    }

    private void Update()
    {
        if (!base.IsServer)
            return;
        if (_tired)
        {
            Vector3 newTarget = transform.position - new Vector3(100, 300, 100);

            transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed * 3, Space.Self);

            Vector3 directionToTarget = newTarget - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            Debug.Log(_target);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _lookAtSpeed * Time.deltaTime);

            return;
        }

        if (_target != null)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed, Space.Self);

            Vector3 directionToTarget = _target.PlayerTransform.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            Debug.Log(_target);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _lookAtSpeed * Time.deltaTime);
        }
    }
}