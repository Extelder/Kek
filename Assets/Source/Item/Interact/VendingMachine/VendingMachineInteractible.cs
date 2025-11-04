using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class VendingMachineInteractible : NetworkBehaviour, IPointerDownHandler
{
    [field: SerializeField] public BuyableItemData ItemData { get; private set; }
    private bool _canInteract = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        Interact();
    }
    
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