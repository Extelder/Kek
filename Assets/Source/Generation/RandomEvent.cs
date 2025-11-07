using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class RandomEvent : NetworkBehaviour
{
    public override void OnStartClient()
    {
        RandomEventsSpawner.Instance.RegisterSpawnedEvent(NetworkObject);
    }
}