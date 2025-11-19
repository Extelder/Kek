using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Pools : NetworkBehaviour
{
    [field: SerializeField] public Pool BloodPool { get; private set; }

    public static Pools Instance { get; private set; }

    private void Start()
    {
        Debug.LogError("### START POOLS: " + gameObject.name + " ### " + this.GetInstanceID());
    }

    public override void OnStartClient()
    {
        Instance = this;
    }
}