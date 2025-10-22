using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pools : MonoBehaviour
{
    public static Pools Instance { get; private set; }
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            return;
        }
        Debug.LogError("Theres one more Pools");
    }
    
    [field:SerializeField] public Pool TrailPool { get; private set; }
    [field:SerializeField] public Pool DefaultProjectilePool { get; private set; }
    [field:SerializeField] public Pool TurretMachineGunProjectilePool { get; private set; }
    [field:SerializeField] public Pool TurretRPGProjectilePool { get; private set; }
    [field:SerializeField] public Pool ExplodeKamikzaePool { get; private set; }
    [field:SerializeField] public Pool FlyEnemyProjectilePool { get; private set; }
    [field:SerializeField] public Pool CoinPool { get; private set; }
    [field:SerializeField] public Pool BloodExplodeDecalPool { get; private set; }
}