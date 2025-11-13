using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class CollectTrigger : NetworkBehaviour
{
    [SerializeField] private Animator _chunkAnimator;

    [SerializeField] private Quota _quota;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<InteractItem>(out InteractItem interactItem))
        {
            if (interactItem.Item is PickUpableItem pickUpableItem)
            {
                PlayerCharacter.Instance.PlayerWallet.Add(pickUpableItem.Price);

                _quota.Add(1);

                _chunkAnimator.SetTrigger("Eat");

                interactItem.Despawn();
            }
        }
    }
}