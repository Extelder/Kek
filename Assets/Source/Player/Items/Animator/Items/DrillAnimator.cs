using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DrillAnimator : MineableItemAnimator
{
    [SerializeField] private Drill _drill;
    
    public override void StartAnimation()
    {
        Animator.DrillAnim();
    }

    public override void OnWeaponVisited(IWeaponVisitor visitor)
    {
        visitor.Visit(_drill, Hit);
    }
}