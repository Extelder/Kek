using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAxeAnimator : ItemAnimator
{
    [SerializeField] private Pickaxe _pickaxe;
    [SerializeField] private RaycastSettings _raycastSettings;
    private RaycastHit _hit;

    public override void Attack()
    {
        bool hitted = Physics.Raycast(_raycastSettings.Origin.position, _raycastSettings.Origin.forward, out _hit,
            _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
        Debug.DrawRay(_raycastSettings.Origin.position, _raycastSettings.Origin.forward * _raycastSettings.MaxDistance,
            Color.red);
        if (hitted)
        {
            if (_hit.collider.TryGetComponent<IWeaponVisitor>(out IWeaponVisitor weaponVisitor))
            {
                weaponVisitor.Visit(_pickaxe, _hit);
            }
        }
    }

    public override void AnimationEndCheck()
    {
        Animator.AttackAnim();
    }
}