using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class CartItemsHolder : NetworkBehaviour
{
    [SerializeField] private Collider _collider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<InteractItem>(out InteractItem InteractItem))
        {
            if (InteractItem.Item is PickUpableItem pickUpableItem)
            {
                SetRigidbodyKinematic(InteractItem.NetworkObject, transform, true);
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetRigidbodyKinematic(NetworkObject obj, Transform parent, bool kinematic)
    {
        Physics.IgnoreCollision(_collider, obj.GetComponent<Collider>(), true);

        SetRigidbodyKinematicObserver(obj, parent, kinematic);
    }

    [ObserversRpc]
    public void SetRigidbodyKinematicObserver(NetworkObject obj, Transform parent, bool kinematic)
    {
        Physics.IgnoreCollision(_collider, obj.GetComponent<Collider>(), true);
        obj.GetComponent<Collider>().isTrigger = true;
        var rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = kinematic;

        obj.transform.SetParent(parent);
        obj.transform.parent = parent;
    }
}