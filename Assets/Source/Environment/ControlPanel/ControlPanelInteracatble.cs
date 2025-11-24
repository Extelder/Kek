using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelInteracatble : Item
{
    [SerializeField] private NetWorkAnimatorSynchronize _netWorkAnimator;
    [SerializeField] private MixSoundAndPlay _audio;

    public override void Interact()
    {
        _netWorkAnimator.SetAnimatorBool("IsActivate", !_netWorkAnimator.Animator.GetBool("IsActivate"));
        if (!_netWorkAnimator.Animator.GetBool("IsActivate"))
        {    
            _audio?.MixOnServer();
        }
    }
}