using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoubleChaseAnimator : EnemyAnimator
{
    [SerializeField] private string _secondRun;
    public void SecondRun()
    {
        SetAnimationBoolAndDisableOther(_secondRun);
    }

    public override void DisableAllBools()
    {
        base.DisableAllBools();
        SetAnimationBool(_secondRun, false);
    }
}
