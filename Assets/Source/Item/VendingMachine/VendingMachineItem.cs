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
        _vendingMachine.SetIsVne(false);
        PlayerCharacter.Instance.SwitchHands();
        Interacted?.Invoke(_targetPoint, _vendingMachine);
    }
}