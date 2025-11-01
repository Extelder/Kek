using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;

[Serializable]
public class PickUpableItem : Item
{
    [field :SerializeField] public InteractItem InteractItem { get; private set; }

    [SerializeField] private ItemData _item;

    public override void Interact()
    {
        if (PlayerCharacter.Instance.PlayerInventory.TryAddItem(_item))
        {
            InteractItem.DespawnObject();
        }
    }
}