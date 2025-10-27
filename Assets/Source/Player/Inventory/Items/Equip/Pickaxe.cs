using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : EquipItem
{
    public override void OnAttackInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.AttackAnim();
    }

    public override void OnAttackInputCanceled(InputAction.CallbackContext obj)
    {
    }
}