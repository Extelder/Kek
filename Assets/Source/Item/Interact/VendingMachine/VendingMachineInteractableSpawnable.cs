using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineInteractableSpawnable : VendingMachineInteractible
{
    [SerializeField] private VendingMachine _vendingMachine;
    public override void OnBought()
    {
        _vendingMachine.Spawn(ItemData);
    }
}