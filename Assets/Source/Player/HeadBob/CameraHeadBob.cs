using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UniRx;
using UnityEngine;

public class CameraHeadBob : NetworkBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Animator _animator;

    private CompositeDisposable _disposable = new CompositeDisposable();

    public override void OnStartClient()
    {
        if (!base.IsOwner)
            return;
        base.OnStartClient();
        _playerMovement.Moving.Subscribe(_ => { SetAnimatorSpeedServer(Convert.ToInt16(_)); }).AddTo(_disposable);
    }

    public void EnableAnimator(bool enabled)
    {
        _animator.enabled = enabled;
        transform.localPosition = new Vector3(0,0,0);
    }

    [ServerRpc]
    public void SetAnimatorSpeedServer(float speed)
    {
        SetAnimatorSpeedObserver(speed);
    }

    [ObserversRpc]
    public void SetAnimatorSpeedObserver(float speed)
    {
        _animator.speed = Convert.ToInt16(speed);
    }

    private void OnDisable()
    {
        if (!base.IsOwner)
            return;
        _disposable.Clear();
    }
}