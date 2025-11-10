using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CigaretteAnimator : ItemAnimator
{
    [SerializeField] private PlayerInventory _inventory;

    public override void Attack()
    {
        _inventory.ClearEquipSlot();
    }

    public override void AnimationEndCheck()
    {
        Animator.SmokeAnim();
    }
}