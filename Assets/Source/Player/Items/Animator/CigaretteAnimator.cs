using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CigaretteAnimator : ItemAnimator
{
    [SerializeField] private float _boostSpeed = 2;
    [SerializeField] private float _timeBoostSpeed = 3;

    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private VolumeLerp _volume;

    [SerializeField] private PlayerInventory _inventory;

    public override void Attack()
    {
        _movement.BoostSpeed(_boostSpeed, _timeBoostSpeed);
        _volume.StartLerp();
        _inventory.ClearEquipSlot();
    }

    public override void AnimationEndCheck()
    {
        Animator.SmokeAnim();
    }
}