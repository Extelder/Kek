using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : MonoBehaviour
{
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private PlayerInventoryItem _inventoryItem;

    private void OnEnable()
    {
        _inventoryItem.ChangeEquipState += OnChangeEquipState;
    }

    private void OnChangeEquipState(bool equiped)
    {
        if (equiped)
        {
            Debug.LogError("Subscribe");
            PlayerCharacter.Instance.Binds.Character.MainShoot.started += OnAttackInputStarted;
            PlayerCharacter.Instance.Binds.Character.MainShoot.canceled += OnAttackInputCanceled;
        }
        else
        {
            _playerAnimator.DisableAllBools();
            Debug.LogError("UnSubscribe");

            PlayerCharacter.Instance.Binds.Character.MainShoot.started -= OnAttackInputStarted;
            PlayerCharacter.Instance.Binds.Character.MainShoot.canceled -= OnAttackInputCanceled;
        }
    }

    private void OnAttackInputCanceled(InputAction.CallbackContext obj)
    {
        Debug.LogError("cancel");
        _playerAnimator.DisableAllBools();
    }

    private void OnAttackInputStarted(InputAction.CallbackContext obj)
    {
        Debug.LogError("start");
        _playerAnimator.AttackAnim();
    }

    private void OnDisable()
    {
        _inventoryItem.ChangeEquipState -= OnChangeEquipState;
        PlayerCharacter.Instance.Binds.Character.MainShoot.started -= OnAttackInputStarted;
        PlayerCharacter.Instance.Binds.Character.MainShoot.canceled -= OnAttackInputCanceled;
    }
}