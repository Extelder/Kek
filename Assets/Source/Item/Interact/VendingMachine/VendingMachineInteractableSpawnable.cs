using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineInteractableSpawnable : VendingMachineInteractible
{
    [SerializeField] private VendingMachine _vendingMachine;

    public event Action<string> TextChanged;
    public event Action<int> PriceChanged;

    public override void OnStartClient()
    {
        base.OnStartClient();
        TextChanged?.Invoke(ItemData.Name);
        PriceChanged?.Invoke(ItemData.Price);
    }

    public override void OnBought()
    {
        _vendingMachine.Spawn(ItemData);
    }
}