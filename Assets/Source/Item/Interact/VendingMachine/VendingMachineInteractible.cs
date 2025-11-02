using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class VendingMachineInteractible : NetworkBehaviour, IInteractable
{
    [field: SerializeField] public BuyableItemData ItemData { get; private set; }

    public void Interact()
    {
        InteractServer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void InteractServer()
    {
        InteractObserver();
    }

    [ObserversRpc]
    public void InteractObserver()
    {
        PlayerCharacter.Instance.Wallet.SpendServer(ItemData.Price);
    }
}