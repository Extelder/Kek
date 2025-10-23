using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class InteractItem : NetworkBehaviour, IInteractable
{
    [SerializeReference] [SerializeReferenceButton]
    public Item Item;

    public virtual void Interact()
    {
        Item.Interact();
    }

    public virtual void Detected()
    {
        Debug.Log("Detected");
    }

    public virtual void Lost()
    {
        Debug.Log("Lost");
    }
}
