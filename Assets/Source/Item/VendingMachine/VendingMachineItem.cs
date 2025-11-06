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
        PlayerCharacter.Instance.SwitchHands();
        Interacted?.Invoke(_targetPoint);
    }
}
