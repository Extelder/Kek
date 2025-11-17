using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTAnimator : ItemAnimator
{
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private GameObject _tntThrowablePrefab;
    [SerializeField] private AudioSource _sound;

    public override void Attack()
    {
        PlayerCharacter.Instance.ServerSpawnObject(_tntThrowablePrefab, PlayerCharacter.Instance.DropPoint.position,
            PlayerCharacter.Instance.CameraTransform.rotation);
        _inventory.ClearEquipSlot();
        _sound.Play();
    }

    public override void AnimationEndCheck()
    {
        Animator.ThrowAnim();
    }
}