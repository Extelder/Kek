using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimatorEventHandler : MonoBehaviour
{
    [SerializeField] private ItemTakeUp _takeUp;
    [SerializeField] private GameObject _tntThrowablePrefab;
    [SerializeField] private AudioSource _music;


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
        if (_currentItemAnimator == null)
            return;
        _music.Play();
        PlayerCharacter.Instance.ServerSpawnObject(_tntThrowablePrefab, PlayerCharacter.Instance.DropPoint.position,
            PlayerCharacter.Instance.CameraTransform.rotation);
    }

    public void Attack()
    {
        _currentItemAnimator?.Attack();
    }
}