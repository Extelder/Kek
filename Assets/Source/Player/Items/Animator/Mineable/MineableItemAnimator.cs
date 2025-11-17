using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public abstract class MineableItemAnimator : ItemAnimator
{
    [SerializeField] private CinemachineImpulseSource _hiitedImpulseSource;

    [SerializeField] private RaycastSettings _raycastSettings;
    public RaycastHit Hit;
    public event Action Hitted;
    public event Action NotHitted;
    
    
    public override void Attack()
    {
        AttackPerfromed?.Invoke();
        bool hitted = Physics.Raycast(_raycastSettings.Origin.position, _raycastSettings.Origin.forward, out Hit,
            _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
        Debug.DrawRay(_raycastSettings.Origin.position, _raycastSettings.Origin.forward * _raycastSettings.MaxDistance,
            Color.red);
        if (hitted)
        {
            if (Hit.collider.TryGetComponent<IWeaponVisitor>(out IWeaponVisitor weaponVisitor))
            {
                OnWeaponVisited(weaponVisitor);
                _hiitedImpulseSource.GenerateImpulse();
                Hitted?.Invoke();
            }
            return;
        }
        NotHitted?.Invoke();
    }

    public override void AnimationEndCheck()
    {
        StartAnimation();
    }

    public abstract void StartAnimation();
    public abstract void OnWeaponVisited(IWeaponVisitor visitor);
}
