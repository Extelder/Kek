using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAnimator : MonoBehaviour
{
    [field: SerializeField] public Animator Animator { get; private set; }

    public abstract void DisableAllBools();

    public virtual void Idle()
    {
        DisableAllBools();
    }

    public void SetAnimationBoolAndDisableOther(string name)
    {
        DisableAllBools();
        SetAnimationBool(name, true);
    }
    
    public void SetAnimationBool(string name, bool value)
    {
        Animator.SetBool(name, value);
    }

    public void SetAnimationTrigger(string name)
    {
        Animator.SetTrigger(name);
    }

    public void ResetAnimationTrigger(string name)
    {
        Animator.ResetTrigger(name);
    }
}
