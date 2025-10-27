using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TNT : EquipItem
{
    public override void OnAttackInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.ThrowAnim();
    }

    public override void OnAttackInputCanceled(InputAction.CallbackContext obj)
    {
    }
}
