using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CigaretteAnimator : ItemAnimator
{
    public override void AnimationEndCheck()
    {
        Animator.SmokeAnim();
    }
}
