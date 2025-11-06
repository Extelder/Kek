using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PickAxeAnimator : ItemAnimator
{
    [SerializeField] private CinemachineImpulseSource _hiitedImpulseSource;

    [SerializeField] private Pickaxe _pickaxe;
    [SerializeField] private RaycastSettings _raycastSettings;
    [SerializeField] private MixSoundAndPlay _mixsound;
    [SerializeField] private MixSoundAndPlay _secondaryMixsound;
    [SerializeField] private AudioSource _secondaryAudioSource;
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
                _hiitedImpulseSource.GenerateImpulse();
                weaponVisitor.Visit(_pickaxe, _hit);
                _mixsound.MixOnServer();
                _secondaryAudioSource.Play();
            }
        }
        else
        {
            _secondaryMixsound.MixOnServer();
        }
    }

    public override void AnimationEndCheck()
    {
        Animator.AttackAnim();
    }
}