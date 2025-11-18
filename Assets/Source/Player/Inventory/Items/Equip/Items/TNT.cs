using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TNT : EquipItem
{
    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.ThrowAnim();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
    }
}
