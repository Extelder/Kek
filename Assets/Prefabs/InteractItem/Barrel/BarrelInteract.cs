using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelInteract : Item
{
    [SerializeField] private BarrelExplosion barrel;
    public override void Interact()
    {
        barrel.Interact();
    }
}
