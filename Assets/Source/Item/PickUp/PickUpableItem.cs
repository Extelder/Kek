using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;

[Serializable]
public class PickUpableItem : Item
{
    [SerializeField] private NetworkBehaviour _networkBehaviour;
    public override void Interact()
    {
        DespawnObject(_networkBehaviour.gameObject);
    }

    public void DespawnObject(GameObject objectToDespawn)
    {
        _networkBehaviour.ServerManager.Despawn(objectToDespawn);
    }
}
