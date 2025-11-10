using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public abstract class RandomEvent : NetworkBehaviour
{
    public abstract void StartEvent();

    public override void OnStartClient()
    {
        if (IsServer)
        {
            StartEvent();
            RandomEventsSpawner.Instance.RegisterSpawnedEvent(NetworkObject);
        }
    }
}