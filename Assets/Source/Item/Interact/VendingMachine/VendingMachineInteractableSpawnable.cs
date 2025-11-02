using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachineInteractableSpawnable : VendingMachineInteractible
{
    public static event Action<BuyableItemData> ItemBought;
    public override void OnBought()
    {
        ItemBought?.Invoke(ItemData);
    }
}
