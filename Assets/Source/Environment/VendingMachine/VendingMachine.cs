using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    [SerializeField] private VendingMachineInteractableSpawnable _vendingMachineInteractible;

    [SerializeField] private Transform _spawnOrigin;

    private void OnEnable()
    {
        _vendingMachineInteractible.ItemBought += OnItemBought;
    }

    private void OnItemBought(BuyableItemData buyableItemData)
    {
        PlayerCharacter.Instance.ServerSpawnObject(buyableItemData.Prefab, _spawnOrigin.position, Quaternion.identity);
    }

    private void OnDisable()
    {
        _vendingMachineInteractible.ItemBought -= OnItemBought;
    }
}