using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : MonoBehaviour
{
    [SerializeField] private PlayerInventoryItem _inventoryItem;

    private void OnEnable()
    {
        _inventoryItem.ChangeEquipState += OnChangeEquipState;
    }

    private void OnChangeEquipState(bool equiped)
    {
        if (equiped)
        {
            PlayerCharacter.Instance.Binds.Character.MainShoot.started += OnAttackInputReceived;
        }
        else
        {
            PlayerCharacter.Instance.Binds.Character.MainShoot.started -= OnAttackInputReceived;
        }
    }

    private void OnAttackInputReceived(InputAction.CallbackContext obj)
    {
    }

    private void OnDisable()
    {
        _inventoryItem.ChangeEquipState -= OnChangeEquipState;
        PlayerCharacter.Instance.Binds.Character.MainShoot.started -= OnAttackInputReceived;
    }
}