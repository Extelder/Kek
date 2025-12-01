using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerSpawnable : NetworkBehaviour
{
    public static event Action<GameObject> Spawned;

    public override void OnStartClient()
    {
        Invoke(nameof(SpawnedWithDelay), 1f);
    }

    private void SpawnedWithDelay()
    {
        Spawned?.Invoke(gameObject);
    }
}