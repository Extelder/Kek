using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInteract : Item
{
    public event Action InteractExplosion;
    [SerializeField] private BoxExplosion box;
    public override void Interact()
    {
        if (!box.IsUse)
        {
            box.Interact();
            InteractExplosion?.Invoke();
        }
    }
}
