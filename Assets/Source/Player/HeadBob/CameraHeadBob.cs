using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UniRx;
using UnityEngine;

public class CameraHeadBob : NetworkBehaviour
{
    [SerializeField] private GroundChecker _checker;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Animator _animator;

    private CompositeDisposable _disposable = new CompositeDisposable();

    public override void OnStartClient()
    {
        if (!base.IsOwner)
            return;
        base.OnStartClient();
        StartCoroutine(ChekingForMovingOnGreound());
    }

    private IEnumerator ChekingForMovingOnGreound()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            int speed = Convert.ToInt16(_playerMovement.Moving.Value);

            if (!_checker.Detected)
                speed = 0;

            SetAnimatorSpeedServer(speed);
        }
    }

    public void EnableAnimator(bool enabled)
    {
        _animator.enabled = enabled;
        transform.localPosition = new Vector3(0, 0, 0);
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
        StopAllCoroutines();
        _disposable.Clear();
    }
}