using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class BarrelExplosion : NetworkBehaviour
{
    [SerializeField] private GameObject _object;
    [SerializeField] private float _strenge;
    private bool IsUse;
    
    [ServerRpc(RequireOwnership = false)]
    public void Interact()
    {
        if (!IsUse)
        {
            IsUse = true;
            InteractObserver();
        }
    }
    [ObserversRpc]
    private void InteractObserver()
    {
        Rigidbody rb = _object.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        Vector3 dir = (Vector3.up * 0.7f) + (Random.insideUnitSphere * 0.45f);
        dir.Normalize();
        float force = _strenge;
        rb.AddForce(dir * force, ForceMode.VelocityChange);
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.VelocityChange);
    }
    
}
