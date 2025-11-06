using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DrillAnimator : ItemAnimator
{
    [SerializeField] private CinemachineImpulseSource _hiitedImpulseSource;

    [SerializeField] private Drill _drill;
    [SerializeField] private RaycastSettings _raycastSettings;
    [SerializeField] private SoundPlayPause _musicSecondary;
    private bool _musicplay;
    private RaycastHit _hit;

    public override void Attack()
    {
        AttackPerfromed?.Invoke();
        bool hitted = Physics.Raycast(_raycastSettings.Origin.position, _raycastSettings.Origin.forward, out _hit,
            _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
        Debug.DrawRay(_raycastSettings.Origin.position, _raycastSettings.Origin.forward * _raycastSettings.MaxDistance,
            Color.red);
        if (hitted)
        {
            if (_hit.collider.TryGetComponent<IWeaponVisitor>(out IWeaponVisitor weaponVisitor))
            {
                weaponVisitor.Visit(_drill, _hit);
                _hiitedImpulseSource.GenerateImpulse();
                if (!_musicplay)
                {
                    _musicSecondary.Pause(false);
                    _musicplay = true;
                }

                return;
            }
        }

        if (_musicplay)
        {
            _musicSecondary.Pause(true);
            _musicplay = false;
        }
    }

    private void OnDisable()
    {
        if (_musicplay)
        {
            _musicSecondary.Pause(true);
            _musicplay = false;
        }
    }

    public override void AnimationEndCheck()
    {
        Animator.DrillAnim();
    }
}