using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Guitar : EquipItem
{
    [SerializeField] private SoundPlayPause _music;

    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        _music.Pause(false); 
        PlayerAnimator.GuitarAnim();
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
        _music.Pause(true); 
        PlayerAnimator.DisableAll();
    }
}
