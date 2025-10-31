using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drill : EquipItem
{
    [SerializeField] private float _checkRate;

    private PickUpableItem _pickUpableItem;
    private CompositeDisposable _disposable = new CompositeDisposable();

    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        Debug.Log("Drill");
        PlayerAnimator.DrillAnim();
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