using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimatorEventHandler : MonoBehaviour
{
    [SerializeField] private ItemTakeUp _takeUp;


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

    public void Attack()
    {
        _currentItemAnimator?.Attack();
    }
}