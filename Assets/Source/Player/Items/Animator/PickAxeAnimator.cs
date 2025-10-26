using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAxeAnimator : ItemAnimator
{
    public override void AnimationEndCheck()
    {
        Animator.AttackAnim();
    }
}