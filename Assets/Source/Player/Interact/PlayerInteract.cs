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
    [SerializeField] private PlayerCharacter _character;
    [SerializeField] private RaycastSettings _raycastSettings;
    private RaycastHit _hit;

    [SerializeField] private float _checkCooldown;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private InteractItem _currentItem;
    private PlayerBinds _binds;
    private IInteractable _nowInteractable;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            return;
        CheckInteractable();
        _binds = PlayerCharacter.Instance.Binds;
        _binds.Character.Interact.started += OnButtonPerformed;
        _binds.Character.Interact.canceled += OnButtonCancelled;
    }

    private void OnButtonCancelled(InputAction.CallbackContext obj)
    {
        if (_nowInteractable != null) _nowInteractable.InteractCancelled();
    }

    private void OnButtonPerformed(InputAction.CallbackContext obj)
    {
        _nowInteractable = null;
        bool hitted = Physics.Raycast(_raycastSettings.Origin.position, _raycastSettings.Origin.forward, out _hit,
            _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
        if (hitted)
        {
            if (_hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                _nowInteractable = interactable;
                interactable.Interact();
            }
            if (_hit.collider.TryGetComponent<PlayerCart>(out PlayerCart PlayerCart))
            {
                PlayerCart.Interact(_character);
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