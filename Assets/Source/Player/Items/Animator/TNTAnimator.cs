using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTAnimator : ItemAnimator
{

    public override void AnimationEndCheck()
    {
        Animator.ThrowAnim();
    }
}
