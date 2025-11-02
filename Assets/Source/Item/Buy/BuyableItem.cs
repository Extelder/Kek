using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuyableItem : Item
{
    [field: SerializeField] public BuyableItemData ItemData { get; private set; }

    public override void Interact()
    {
        PlayerCharacter.Instance.Wallet.SpendBithc(ItemData.Price);
        OnBought();
    }

    public abstract void OnBought();
}