using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillAnimator : ItemAnimator
{
    public override void AnimationEndCheck()
    {
        Animator.DrillAnim();
    }
}
