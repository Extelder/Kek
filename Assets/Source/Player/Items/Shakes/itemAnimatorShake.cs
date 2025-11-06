using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class itemAnimatorShake : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _cinemachineImpulse;

    [SerializeField] private ItemAnimator _itemAnimator;

    private void OnEnable()
    {
        _itemAnimator.AttackPerfromed += OnAttackPerfromed;
    }

    private void OnAttackPerfromed()
    {
        _cinemachineImpulse.GenerateImpulse();
    }

    private void OnDisable()
    {
        _itemAnimator.AttackPerfromed -= OnAttackPerfromed;
    }
}