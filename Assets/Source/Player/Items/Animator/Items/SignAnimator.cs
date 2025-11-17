using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using UnityEngine;

public class SignAnimator : ItemAnimator
{
    [SerializeField] private GameObject _sign;
    [SerializeField] private RaycastSettings _raycastSettings;

    private RaycastHit _hit;
    public override void Attack()
    {
        bool hitted = Physics.Raycast(_raycastSettings.Origin.position, _raycastSettings.Origin.forward, out _hit,
            _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
        Debug.DrawRay(_raycastSettings.Origin.position, _raycastSettings.Origin.forward * _raycastSettings.MaxDistance,
            Color.red, 5);
        if (hitted)
        {
            if (_hit.collider.TryGetComponent<Ground>(out Ground ground))
            {
                PlayerCharacter.Instance.ServerSpawnObject(_sign, _hit.point, Quaternion.LookRotation(-_hit.normal));
            }
        }
    }

    public override void AnimationEndCheck()
    {
        Animator.SignAnim();
    }
}
