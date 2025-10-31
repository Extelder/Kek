using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimatorEventHandler : MonoBehaviour
{
    [SerializeField] private ItemTakeUp _takeUp;
    [SerializeField] private GameObject _tntThrowablePrefab;
    [SerializeField] private RaycastSettings _raycastSettings;
    private RaycastHit _hit;
    private ItemAnimator _currentItemAnimator;

    public void ChooseItemAnimator(ItemAnimator itemAnimator)
    {
        AnimationEndWithoutChecking();
        _takeUp.TakeUp();
        _currentItemAnimator = itemAnimator;
    }

    public void AnimationEndStartChecking()
    {
        _currentItemAnimator?.AnimationEndStartChecking();
    }
    
    public void AnimationEndWithoutChecking()
    {
        _currentItemAnimator?.AnimationEndWithoutChecking();
    }

    public void AnimationEndStopChecking()
    {
        _currentItemAnimator?.AnimationEndStopChecking();
    }

    public void ThrowTNT()
    {
        PlayerCharacter.Instance.ServerSpawnObject(_tntThrowablePrefab, PlayerCharacter.Instance.DropPoint.position, 
            PlayerCharacter.Instance.CameraTransform.rotation);
    }

    public void Attack()
    {
        bool hitted = Physics.Raycast(_raycastSettings.Origin.position, _raycastSettings.Origin.forward, out _hit,
            _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
        Debug.DrawRay(_raycastSettings.Origin.position, _raycastSettings.Origin.forward * _raycastSettings.MaxDistance, Color.red);
        if (hitted)
        {
            if (_hit.collider.TryGetComponent<InteractItem>(out InteractItem interactItem))
            {
                if (interactItem.Item is MineableItem)
                {
                    interactItem.Interact();
                }
            }
        }
    }
}
