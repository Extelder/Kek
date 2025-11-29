using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineInteractableSpawnable : VendingMachineInteractible
{
    public event Action<string, int, Sprite> DataChanged;

    public override void OnStartClient()
    {
        base.OnStartClient();
        DataChanged?.Invoke(ItemData.Name, ItemData.Price, ItemData.Image);
    }

    public override void OnBought()
    {
        VendingMachine.GetCurrentBuyable(ItemData);
    }
}