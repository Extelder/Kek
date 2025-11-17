using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PickAxeAnimator : MineableItemAnimator
{
    [SerializeField] private Pickaxe _pickaxe;
    
    public override void StartAnimation()
    {
        Animator.AttackAnim();
    }

    public override void OnWeaponVisited(IWeaponVisitor visitor)
    {
        visitor.Visit(_pickaxe, Hit);
    }
}