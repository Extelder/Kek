using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RPG : EquipItem
{
    [SerializeField] private Vector3 _cameraTransformValue;
    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.RPGShootAnim();
    }

    public override void OnEquipStateChanged()
    {
        SetCameraTransformValue(_cameraTransformValue);
        PlayerAnimator.RPGTakeAnim();
    }

    public override void OnUnEquiped()
    {
        base.OnUnEquiped();
        PlayerAnimator.RPGTakeStopAnim();
        SetCameraTransformDefaultValue();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
        PlayerAnimator.DisableAll();
    }
}
