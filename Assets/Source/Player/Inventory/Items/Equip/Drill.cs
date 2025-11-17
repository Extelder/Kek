using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drill : EquipItem
{
    public event Action<bool> StartedDrilling;
    
    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.DrillAnim();
        StartedDrilling?.Invoke(false);
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
        PlayerAnimator.DisableAll();
        StartedDrilling?.Invoke(true);
    }
}