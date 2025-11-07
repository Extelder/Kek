using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class RandomEvent : NetworkBehaviour
{
    public override void OnStartClient()
    {
        if (IsServer)
            RandomEventsSpawner.Instance.RegisterSpawnedEvent(NetworkObject);
    }
}