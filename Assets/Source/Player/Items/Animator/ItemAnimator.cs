using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class ItemAnimator : MonoBehaviour
{
    public bool CanShoot = true;
    public ReactiveProperty<bool> AlreadyShooting = new ReactiveProperty<bool>();
    
    public void AnimationEndStartChecking()
    {
        AlreadyShooting.Value = false;
        StopAllCoroutines();
        StartCoroutine(AnimationEndChecking());
    }

    public void AnimationEndWithoutChecking()
    {
        StopAllCoroutines();

        //CanChanged = true;
    }

    public void AnimationEndStopChecking()
    {
        StopAllCoroutines();

        if (AlreadyShooting.Value)
            return;

        //CanChanged = true;
    }

    public virtual IEnumerator AnimationEndChecking()
    {
        while (true)
        {
            if (!CanShoot)
            {
                //CanChanged = true;
                yield break;
            }

            if (PlayerCharacter.Instance.Binds.Character.MainShoot.inProgress)
            {
                AlreadyShooting.Value = true;
                //Animator.Shoot();
                yield break;
            }

            yield return new WaitForSeconds(0.02f);
        }
    }
}
