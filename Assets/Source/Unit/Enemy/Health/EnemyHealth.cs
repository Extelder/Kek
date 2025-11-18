using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private NetworkObject _object;
    public override void Death()
    {
        _object.Despawn();
    }
}
