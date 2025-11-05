using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class VendingMachineInteractible : NetworkBehaviour, IPointerDownHandler
{
    [field: SerializeField] public BuyableItemData ItemData { get; private set; }
    [field:SerializeField] public VendingMachine VendingMachine;

    public void OnPointerDown(PointerEventData eventData)
    {
        Interact();
    }
    
    public void Interact()
    {
        if (PlayerCharacter.Instance.Wallet.TryBuy(ItemData.Price) && VendingMachine.CanInteract)
        {
            OnBought();
            InteractServer();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InteractServer()
    {
        InteractObserver();
    }

    [ObserversRpc]
    public void InteractObserver()
    {
        PlayerCharacter.Instance.Wallet.Spend(ItemData.Price);
    }

    public abstract void OnBought();
}