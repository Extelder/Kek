using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachineItem : Item
{
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private VendingMachine _vendingMachine;
    public static event Action<Transform, VendingMachine> Interacted;
    
    public override void Interact()
    {
        if (!_vendingMachine.CanInteract)
            return;
        PlayerCharacter.Instance.SwitchHands();
        _vendingMachine.SetInteractBool(false);
        Interacted?.Invoke(_targetPoint, _vendingMachine);
    }
}
