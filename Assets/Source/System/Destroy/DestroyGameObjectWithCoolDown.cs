using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyGameObjectWithCoolDown : NetworkBehaviour
{
    [SerializeField] private float _cooldown;
    
    private void OnEnable()
    {
        StartCoroutine(Destroying());
    }

    private IEnumerator Destroying()
    {
        yield return new WaitForSeconds(_cooldown);
        Destroy();
    }

    [ServerRpc(RequireOwnership = false)]
    private void Destroy()
    {
        DestroyObserverer();
    }

    [ObserversRpc]
    private void DestroyObserverer()
    {
        Despawn();
    }
}
