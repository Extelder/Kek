using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimatorEventHandler : MonoBehaviour
{
    private ItemAnimator _currentItemAnimator;

    public void ChooseItemAnimator(ItemAnimator itemAnimator)
    {
        _currentItemAnimator = itemAnimator;
    }

    public void AnimationEndStartChecking()
    {
        _currentItemAnimator?.AnimationEndStartChecking();
    }
    
    public void AnimationEndWithoutChecking()
    {
        _currentItemAnimator?.AnimationEndStartChecking();
    }

    public void AnimationEndStopChecking()
    {
        _currentItemAnimator?.AnimationEndStopChecking();
    }
}
