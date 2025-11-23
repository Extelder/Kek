using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Guitar : EquipItem
{
    public event Action<bool> StartedPlayingMusic;
    public event Action StoppedPlayingMusic;

    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        StartedPlayingMusic?.Invoke(false);
        PlayerAnimator.GuitarAnim();
    }
    
    
    public override void OnEquipmentNull()
    {
        base.OnEquipmentNull();
        StoppedPlayingMusic?.Invoke();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
        StartedPlayingMusic?.Invoke(true);
        PlayerAnimator.DisableAll();
    }
}
