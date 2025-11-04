using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : EquipItem
{
    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.SignAnim();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
    }
}
