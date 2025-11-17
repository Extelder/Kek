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
        if (other.TryGetComponent<OreQuota>(out OreQuota OreQuota))
        {
            if (OreQuota.InteractItem.Item is PickUpableItem item)
            {
                PlayerCharacter.Instance.PlayerWallet.Add(item.Price);
                _quota.Add(1);
                _chunkAnimator.SetTrigger("Eat");

                OreQuota.Despawn();
            }
        }
    }
}