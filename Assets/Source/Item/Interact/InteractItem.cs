using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class InteractItem : NetworkBehaviour, IInteractable
{
    [SerializeReference] [SerializeReferenceButton]
    public Item Item;

    public event Action Interacted;

    public virtual void Interact()
    {
        Item.Interact();
        Interacted?.Invoke();
    }

    public void InteractCancelled()
    {
        Item.InteractCancelled();
    }

    public virtual void Detected()
    {
    }

    public virtual void Lost()
    {
    }


    [ServerRpc(RequireOwnership = false)]
    public void DespawnObject()
    {
        Despawn();
    }
}