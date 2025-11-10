using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cigarette : EquipItem
{
    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.SmokeAnim();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
    }
}
