using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAnimator : UnitAnimator
{
    [SerializeField] private string _moveAnimationBoolName, _runAnimationBoolName, _attackAnimationBoolName, _kiteAnimationBoolName;
    [SerializeField] private bool _randomizeAttack;
    [ShowIf(nameof(_randomizeAttack)), SerializeField] private string _attackIntName;
    [ShowIf(nameof(_randomizeAttack)), SerializeField] private int _maxAttackTypes;
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
        if(_randomizeAttack)
            RandomizeAttackAnimation();
        SetAnimationBoolAndDisableOther(_attackAnimationBoolName);
        _enemysound.Attack();
    }
    
    public void RandomizeAttackAnimation()
    {
        int value = Random.Range(0, _maxAttackTypes);
        SetAnimationInt(_attackIntName, value);
    }

    public void Kait()
    {
        SetAnimationBoolAndDisableOther(_kiteAnimationBoolName);
    }
}