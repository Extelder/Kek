using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractItem : MonoBehaviour, IInteractable
{
    [SerializeReference] [SerializeReferenceButton]
    public Item Item;

    private bool _detected;

    public void Interact()
    {
        Item.Interact();
    }

    public void Detected()
    {
    }

    public void Lost()
    {
    }
}
