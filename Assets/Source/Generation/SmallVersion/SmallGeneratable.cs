using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class SmallGeneratable : NetworkBehaviour
{
    [field: SerializeField] public Transform[] SpawnPoints { get; private set; }

    public static Action<SmallGeneratable> Spawned;

    public override void OnStartClient()
    {
        if (!base.IsServer)
            return;
        Spawned?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!base.IsServer)
            return;
        if (other.TryGetComponent<SmallGeneratable>(out SmallGeneratable SmallGeneratable))
        {
            if (SmallGeneratable != this)
                Despawn();
        }
    }
}