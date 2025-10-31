using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drill : EquipItem
{
    [SerializeField] private RaycastSettings _raycastSettings;
    [SerializeField] private float _checkRate;
    
    private PickUpableItem _pickUpableItem;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private RaycastHit _hit;

    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        Debug.Log("Drill");
        PlayerAnimator.DrillAnim();
        Observable.Interval(TimeSpan.FromSeconds(_checkRate)).Subscribe(_ =>
        {
            bool hitted = Physics.Raycast(_raycastSettings.Origin.position, _raycastSettings.Origin.forward, out _hit,
                _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
            Debug.DrawRay(_raycastSettings.Origin.position, _raycastSettings.Origin.forward * _raycastSettings.MaxDistance, Color.red);
            if (hitted)
            {
                if (_hit.collider.TryGetComponent<InteractItem>(out InteractItem interactItem))
                {
                    if (interactItem.Item is MineableItem)
                    {
                        interactItem.Interact();
                    }
                }
            }
        }).AddTo(_disposable);
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
        PlayerAnimator.DisableAll(); 
        _disposable.Clear();
    }

    public override void OnDisableVirtual()
    {
        base.OnDisableVirtual();
        _disposable.Clear();
    }
}
