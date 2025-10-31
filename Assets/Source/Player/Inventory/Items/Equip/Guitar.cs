using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Guitar : EquipItem
{
    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.GuitarAnim();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
        //PlayerAnimator.DisableAll();
    }
}
