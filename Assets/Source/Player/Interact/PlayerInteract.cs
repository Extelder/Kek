using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using FishNet.Object;
using UniRx;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private RaycastSettings _raycastSettings;

    [SerializeField] private float _checkCooldown;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private RaycastHit _hit;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
            CheckInteractable();
    }

    private void CheckInteractable()
    {
        Observable.Interval(TimeSpan.FromSeconds(_checkCooldown)).Subscribe(_ =>
        {
            bool hitted = Physics.Raycast(_raycastSettings.Origin.position, Vector3.forward, out _hit,
                _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
            if (hitted)
            {
                if (_hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    interactable.Interact();
                }
            }
        }).AddTo(_disposable);
    }

    private void OnDisable()
    {
        if (base.IsOwner)
            _disposable.Clear();
    }
}