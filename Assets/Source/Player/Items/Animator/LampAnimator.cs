using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampAnimator : ItemAnimator
{
    public override void AnimationEndCheck()
    {
        Animator.PickUpAnim();
    }
}
