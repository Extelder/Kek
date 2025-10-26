using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class ItemAnimator : MonoBehaviour
{
    [field: SerializeField] public PlayerAnimator Animator { get; private set; }
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

        Animator.DisableAllBools();
    }

    private IEnumerator AnimationEndChecking()
    {
        while (true)
        {
            if (!CanUse)
            {
                Animator.DisableAllBools();
                yield break;
            }
            if (PlayerCharacter.Instance.Binds.Character.MainShoot.inProgress)
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
        
        Animator.DisableAllBools();
    }

    public abstract void AnimationEndCheck();
}
