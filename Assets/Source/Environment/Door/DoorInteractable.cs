using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Item
{
    [SerializeField] private NetWorkAnimatorSynchronize _netWorkAnimator;

    public override void Interact()
    {
        _netWorkAnimator.SetAnimatorBool("IsOpen", !_netWorkAnimator.Animator.GetBool("IsOpen"));
    }
}