using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyableAndSpawnableItem : BuyableItem
{
    public static event Action<BuyableItemData> ItemBought;
    public override void OnBought()
    {
        ItemBought?.Invoke(ItemData);
    }
}
