using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lamp : EquipItem
{
    [SerializeField] private GameObject _lampLight;
    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        _lampLight.SetActive(!_lampLight.activeSelf);
    }

    public override void OnEquipStateChanged()
    {
        base.OnEquipStateChanged();
        PlayerAnimator.PickUpAnim();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
    }
}
