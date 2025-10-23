using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using FishNet.Object;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private RaycastSettings _raycastSettings;
    private RaycastHit _hit;

    [SerializeField] private float _checkCooldown;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private InteractItem _currentItem;
    private PlayerBinds _binds;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            return;
        CheckInteractable();
        _binds = PlayerCharacter.Instance.Binds;
        _binds.Character.Interact.started += OnButtonPerformed;
    }

    private void OnButtonPerformed(InputAction.CallbackContext obj)
    {
        bool hitted = Physics.Raycast(_raycastSettings.Origin.position, _raycastSettings.Origin.forward, out _hit,
            _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
        if (hitted)
        {
            if (_hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.Interact();
            }
        }
    }

    private void CheckInteractable()
    {
        Observable.Interval(TimeSpan.FromSeconds(_checkCooldown)).Subscribe(_ =>
        {
            bool hitted = Physics.Raycast(_raycastSettings.Origin.position, _raycastSettings.Origin.forward, out _hit,
                _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
            if (hitted)
            {
                if (_hit.collider.TryGetComponent<InteractItem>(out InteractItem interactItem))
                {
                    _currentItem = interactItem;
                    _currentItem.Detected();
                    return;
                }
            }

            if (_currentItem == null)
                return; 
            _currentItem.Lost();
            _currentItem = null;
        }).AddTo(_disposable);
    }

    private void OnDisable()
    {
        if (!base.IsOwner)
            return;
        _disposable.Clear();
        _binds.Character.Interact.started -= OnButtonPerformed;
    }
}