using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Item
{
    [SerializeField] private NetWorkAnimatorSynchronize _netWorkAnimator;
    [SerializeField] private MixSoundAndPlay _mix;

    public override void Interact()
    {
        _netWorkAnimator.SetAnimatorBool("IsOpen", !_netWorkAnimator.Animator.GetBool("IsOpen"));
        _mix.MixOnServer();
    }
}