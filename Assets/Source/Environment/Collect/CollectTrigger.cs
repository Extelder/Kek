using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class CollectTrigger : NetworkBehaviour
{
    public event Action ItemEatable;
    [SerializeField] private Animator _chunkAnimator;

    [SerializeField] private Quota _quota;
    [SerializeField] private AudioSource _eatAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<OreQuota>(out OreQuota OreQuota))
        {
            if (OreQuota.InteractItem.Item is PickUpableItem item)
            {
                PlayerCharacter.Instance.PlayerWallet.Add(item.Price);
                _quota.Add(50);
                _chunkAnimator.SetTrigger("Eat");
                ItemEatable?.Invoke();
                _eatAudio.Play();

                if (OreQuota != null)
                    OreQuota.Despawn();
            }
        }
    }
}