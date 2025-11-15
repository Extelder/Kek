using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInteract : Item
{
    [SerializeField] private BoxExplosion box;
    public override void Interact()
    {
        box.Interact();
    }
}
