using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public abstract class VendingMachineInteractible : NetworkBehaviour, IInteractable
{
    [field: SerializeField] public BuyableItemData ItemData { get; private set; }
    private bool _canInteract = true;

    public void Interact()
    {
        if (PlayerCharacter.Instance.Wallet.TryBuy(ItemData.Price) && _canInteract)
        {
            OnBought();
            InteractServer();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InteractServer()
    {
        _canInteract = false;
        InteractObserver();
    }

    [ObserversRpc]
    public void InteractObserver()
    {
        PlayerCharacter.Instance.Wallet.Spend(ItemData.Price);
        _canInteract = true;
    }

    public abstract void OnBought();
}