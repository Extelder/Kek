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
    [field :SerializeField] public int Price { get; private set; }
    [SerializeField] private GameObject _SoundSpawn;
    [SerializeField] private ItemData _item;
    [SerializeField] private Transform transform;

    public override void Interact()
    {
        if (PlayerCharacter.Instance.PlayerInventory.TryAddItem(_item))
        {
            if (_SoundSpawn != null && transform != null)
                PlayerCharacter.Instance.ServerSpawnObject(_SoundSpawn, transform.position, transform.rotation);
            InteractItem.DespawnObject();
        }
    }
}