using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : EquipItem
{
    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.AttackAnim();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
    }
}