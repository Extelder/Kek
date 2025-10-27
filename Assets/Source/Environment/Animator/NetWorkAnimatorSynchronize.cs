using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using JetBrains.Annotations;
using UnityEngine;

public class NetWorkAnimatorSynchronize : NetworkBehaviour
{
    private bool _blocked;

    [field: SerializeField] public Animator Animator { get; private set; }

    [ServerRpc(RequireOwnership = false)]
    public void SetAnimatorBool(string name, bool value)
    {
        if (_blocked)
            return;
        SetAnimatorBoolMulticast(name, value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetBlock(bool value)
    {
        SetBlockObservers(value);
    }

    [ObserversRpc]
    private void SetBlockObservers(bool value)
    {
        _blocked = value;
    }

    [ObserversRpc]
    public void SetAnimatorBoolMulticast(string name, bool value)
    {
        Animator.SetBool(name, value);
    }
}