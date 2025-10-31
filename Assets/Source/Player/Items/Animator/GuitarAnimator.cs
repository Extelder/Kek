using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuitarAnimator : ItemAnimator
{
    public override void AnimationEndCheck()
    {
        Animator.GuitarAnim();
    }
}
