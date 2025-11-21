using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public abstract class UnitAnimator : NetworkBehaviour
{
    [field: SerializeField] public Animator Animator { get; private set; }

    public abstract void DisableAllBools();

    public virtual void Idle()
    {
        if (!base.IsServer)
            return;
        DisableAllBools();
    }

    public void SetAnimationBoolAndDisableOther(string name)
    {
        if (!base.IsServer)
            return;
        DisableAllBools();
        SetAnimationBool(name, true);
    }

    public void SetAnimationBool(string name, bool value)
    {
        if (!base.IsServer)
            return;
        Animator.SetBool(name, value);
    }

    public void SetAnimationInt(string name, int value)
    {
        if (!base.IsServer)
            return;
        Animator.SetInteger(name, value);
    }

    public void SetAnimationTrigger(string name)
    {
        if (!base.IsServer)
            return;
        Animator.SetTrigger(name);
    }

    public void ResetAnimationTrigger(string name)
    {
        if (!base.IsServer)
            return;
        Animator.ResetTrigger(name);
    }
}