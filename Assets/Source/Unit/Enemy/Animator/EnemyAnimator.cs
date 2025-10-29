using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : UnitAnimator
{
    [SerializeField] private string _moveAnimationBoolName, _runAnimationBoolName, _attackAnimationBoolName;

    private void Start()
    {
        Idle();
    }

    public override void DisableAllBools()
    {
        SetAnimationBool(_moveAnimationBoolName, false);
        SetAnimationBool(_attackAnimationBoolName, false);
        SetAnimationBool(_runAnimationBoolName, false);
    }

    public void Move()
    {
        SetAnimationBoolAndDisableOther(_moveAnimationBoolName);
    }

    public void Run()
    {
        SetAnimationBoolAndDisableOther(_runAnimationBoolName);
    }

    public void Attack()
    {
        SetAnimationBoolAndDisableOther(_attackAnimationBoolName);
    }
}