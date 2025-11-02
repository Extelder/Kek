using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachineInteractible : MonoBehaviour, IInteractable
{
    [field: SerializeField] public BuyableItemData ItemData { get; private set; }

    public void Interact()
    {
        PlayerCharacter.Instance.Wallet.SpendServer(ItemData.Price);
    }
}