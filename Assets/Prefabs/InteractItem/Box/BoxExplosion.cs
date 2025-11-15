using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class BoxExplosion : NetworkBehaviour
{
    [SerializeField] private GameObject[] _object;
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
        for (int i = 0; i < _object.Length; i++)
        {
            Rigidbody rb = _object[i].GetComponent<Rigidbody>();
            Vector3 dir = (Vector3.up * 0.3f) + (Random.insideUnitSphere * 0.2f);
            dir.Normalize();
            float force = _strenge;
            rb.AddForce(dir * force, ForceMode.VelocityChange);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.VelocityChange);
        }
    }
    
}
