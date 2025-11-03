using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using Observable = UniRx.Observable;

public class PlayerTransitionHands : MonoBehaviour
{
    [SerializeField] private Transform _cameraOrigin;
    [SerializeField] private float _speed;

    [SerializeField] private Animator _animator;
    [SerializeField] private string _triggerName;
    
    private CompositeDisposable _disposable = new CompositeDisposable();
    
    private void OnEnable()
    {
        VendingMachineItem.Interacted += OnInteracted;
    }

    private void OnInteracted(Transform target)
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            _cameraOrigin.position = Vector3.MoveTowards(_cameraOrigin.position, target.position, _speed);
        }).AddTo(_disposable);
        _animator.SetTrigger(_triggerName);
    }

    private void OnDisable()
    {
        VendingMachineItem.Interacted -= OnInteracted;
        _disposable.Clear();
    }
}
