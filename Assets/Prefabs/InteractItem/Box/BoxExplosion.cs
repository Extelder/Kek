using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class BoxExplosion : NetworkBehaviour
{
    [SerializeField] private GameObject[] _object;
    [SerializeField] private GameObject[] _objectDestroyAfterMain;
    [SerializeField] private float _strenge;
    [SerializeField] private bool destroyAll;
    [SerializeField] private bool _lockInteractAfterUse = true;
    private bool IsUse;
    private int _currentIndex;

    [ServerRpc(RequireOwnership = false)]
    public void Interact()
    {
        if (!IsUse)
        {
            InteractObserver();
        }
    }

    [ObserversRpc]
    private void InteractObserver()
    {
        if (!destroyAll)
        {
            Explosion(_currentIndex);
            if (_lockInteractAfterUse)
            {
                _currentIndex++;
            }
            if (_object.Length == _currentIndex)
            {
                if (_lockInteractAfterUse)
                {
                    IsUse = true;
                    GetComponent<Collider>().enabled = false;
                }

                if (_objectDestroyAfterMain.Length > 0)
                {
                    for (int i = 0; i < _objectDestroyAfterMain.Length; i++)
                    {
                        ExplosionAfter(i);
                    }
                }
            }

            return;
        }

        IsUse = true;
        GetComponent<Collider>().enabled = false;
        for (int i = 0; i < _object.Length; i++)
        {
            Explosion(i);
        }
    }

    private void Explosion(int index)
    {
        Rigidbody rb = _object[index].GetComponent<Rigidbody>();
        rb.isKinematic = false;
        Vector3 dir = (Vector3.up * 0.3f) + (Random.insideUnitSphere * 0.2f);
        dir.Normalize();
        float force = _strenge;
        rb.AddForce(dir * force, ForceMode.VelocityChange);
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.VelocityChange);
    }

    private void ExplosionAfter(int index)
    {
        Rigidbody rb = _objectDestroyAfterMain[index].GetComponent<Rigidbody>();
        rb.isKinematic = false;
        Vector3 dir = (Vector3.up * 0.3f) + (Random.insideUnitSphere * 0.2f);
        dir.Normalize();
        float force = _strenge;
        rb.AddForce(dir * force, ForceMode.VelocityChange);
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.VelocityChange);
    }
}