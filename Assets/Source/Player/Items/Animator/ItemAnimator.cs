using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class ItemAnimator : MonoBehaviour
{
    [field: SerializeField] public PlayerAnimator Animator { get; private set; }
    
    [SerializeField] private string _actionName;

    [HideInInspector] public bool CanUse = true;
    [HideInInspector] public bool AlreadyUsing;
    
    public void AnimationEndStartChecking()
    {
        AlreadyUsing = false;
        StopAllCoroutines();
        StartCoroutine(AnimationEndChecking());
    }

    public void AnimationEndWithoutChecking()
    {
        StopAllCoroutines();
        
        Animator.DisableAll();
    }

    public void AnimationReset()
    {
        StopAllCoroutines();
        
        Animator.ResetAnim();
    }

    private IEnumerator AnimationEndChecking()
    {
        while (true)
        {
            if (!CanUse)
            {
                Animator.DisableAll();
                yield break;
            }
            if (PlayerCharacter.Instance.Binds.FindAction(_actionName, true).inProgress)
            {
                AlreadyUsing = true;
                AnimationEndCheck();
                yield break;
            }
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void AnimationEndStopChecking()
    {
        StopAllCoroutines();

        if (AlreadyUsing)
            return;
        
        Animator.DisableAll();
    }

    public abstract void AnimationEndCheck();
}
