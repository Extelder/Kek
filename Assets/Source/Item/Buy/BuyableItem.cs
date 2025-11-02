using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuyableItem : Item
{
    [field: SerializeField] public BuyableItemData ItemData { get; private set; }
    public override void Interact()
    {
        PlayerCharacter.Instance.Wallet.Spend(ItemData.Price);
        if (PlayerCharacter.Instance.Wallet.AlredySpend)
        {
            OnBought();
            PlayerCharacter.Instance.Wallet.AlredySpend = false;
        }
    }

    public abstract void OnBought();
}
