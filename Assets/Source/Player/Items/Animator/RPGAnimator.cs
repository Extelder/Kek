using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGAnimator : ItemAnimator
{
    public override void AnimationEndCheck()
    {
        Animator.RPGShootAnim();
    }
}
