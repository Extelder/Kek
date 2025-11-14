using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGAnimator : ItemAnimator
{
    [SerializeField] private GameObject _spawnableObject;
    [SerializeField] private Transform _spawnOrigin;
    public override void AnimationEndCheck()
    {
        Animator.RPGShootAnim();
    }

    public override void Attack()
    {
        base.Attack();
        PlayerCharacter.Instance.ServerSpawnObject(_spawnableObject, _spawnOrigin.position, Quaternion.LookRotation(_spawnOrigin.forward));
    }
}
