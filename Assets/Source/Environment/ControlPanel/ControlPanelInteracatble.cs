using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelInteracatble : Item
{
    [SerializeField] private NetWorkAnimatorSynchronize _netWorkAnimator;

    public override void Interact()
    {
        _netWorkAnimator.SetAnimatorBool("IsActivate", !_netWorkAnimator.Animator.GetBool("IsActivate"));
    }
}