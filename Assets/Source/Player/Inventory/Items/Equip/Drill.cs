using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drill : EquipItem
{
    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.DrillAnim();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
    }
}
