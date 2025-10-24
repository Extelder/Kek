using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using JetBrains.Annotations;
using UnityEngine;

public class NetWorkAnimatorSynchronize : NetworkBehaviour

{
    [field: SerializeField] public Animator Animator { get; private set;}

    [ServerRpc(RequireOwnership = false)]
    public void SetAnimatorBool(string name, bool value)
    {
     SetAnimatorBoolMulticast(name, value);
    }
    [ObserversRpc]
    public void SetAnimatorBoolMulticast(string name, bool value)
    {
        Animator.SetBool(name,value);
    }
}
