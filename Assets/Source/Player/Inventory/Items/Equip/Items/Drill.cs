using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drill : DamageableEquipItem
{
    public event Action<bool> StartedDrilling;
    public event Action StoppedDrilling;
    
    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.DrillAnim();
        StartedDrilling?.Invoke(false);
    }

    public override void OnEquipmentNull()
    {
        base.OnEquipmentNull();
        StoppedDrilling?.Invoke();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
        PlayerAnimator.DisableAll();
        StartedDrilling?.Invoke(true);
    }
}