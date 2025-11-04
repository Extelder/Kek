using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    [SerializeField] private Transform _spawnOrigin;

    public void Spawn(BuyableItemData buyableItemData)
    {
        PlayerCharacter.Instance.ServerSpawnObject(buyableItemData.Prefab, _spawnOrigin.position, Quaternion.identity);
    }
}