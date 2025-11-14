using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class HealInteractItem : Item
{
    [SerializeField] private NetworkObject _networkObject;
    [SerializeField] private float _healValue;

    public override void Interact()
    {
        PlayerCharacter.Instance.PlayerHealth.Heal(_healValue);
        _networkObject.Despawn();
    }
}