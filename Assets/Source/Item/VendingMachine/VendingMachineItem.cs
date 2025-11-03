using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachineItem : Item
{
    [SerializeField] private Transform _targetPoint;

    public static event Action<Transform> Interacted;
    public override void Interact()
    {
        PlayerCharacter.Instance.Hands.SetActive(false);
        PlayerCharacter.Instance.TransitHands.SetActive(true);
        Interacted?.Invoke(_targetPoint);
    }
}
