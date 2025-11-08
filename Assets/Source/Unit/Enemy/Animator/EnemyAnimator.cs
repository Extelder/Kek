using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : UnitAnimator
{
    [SerializeField] private string _moveAnimationBoolName, _runAnimationBoolName, _attackAnimationBoolName, _kiteAnimationBoolName;
    [SerializeField] private EnemySound _enemysound;

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
        _enemysound.Move();
    }

    public void Run()
    {
        SetAnimationBoolAndDisableOther(_runAnimationBoolName);
        _enemysound.Run();
    }

    public void Attack()
    {
        SetAnimationBoolAndDisableOther(_attackAnimationBoolName);
        _enemysound.Attack();
    }

    public void Kait()
    {
        SetAnimationBoolAndDisableOther(_kiteAnimationBoolName);
    }
}