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
    [SerializeField] private ItemData _item;
    [SerializeField] private InteractItem _interactItem;

    public override void Interact()
    {
        if (PlayerCharacter.Instance.PlayerInventory.TryAddItem(_item))
        {
            _interactItem.DespawnObject();
        }
    }
}