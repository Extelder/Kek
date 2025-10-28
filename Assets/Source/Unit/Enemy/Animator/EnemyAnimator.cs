using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : UnitAnimator
{
    [SerializeField] private string _moveAnimationBoolName, _attackAnimationBoolName;

    private void Start()
    {
        Idle();
    }

    public override void DisableAllBools()
    {
        SetAnimationBool(_moveAnimationBoolName, false);
        SetAnimationBool(_attackAnimationBoolName, false);
    }

    public void Move()
    {
        SetAnimationBoolAndDisableOther(_moveAnimationBoolName);
    }

    public void Attack()
    {
        SetAnimationBoolAndDisableOther(_attackAnimationBoolName);
    }
}
