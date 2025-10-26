using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TNT : MonoBehaviour
{
    [SerializeField] private PlayerInventoryItem _inventoryItem;
    [SerializeField] private PlayerAnimator _playerAnimator;

    [SerializeField] private GameObject _tntThrowablePrefab;

    private void OnEnable()
    {
        _inventoryItem.ChangeEquipState += OnChangeEquipState;
    }

    private void OnChangeEquipState(bool equiped)
    {
        if (equiped)
        {
            PlayerCharacter.Instance.Binds.Character.MainShoot.started += OnAttackInputReceived;
            PlayerCharacter.Instance.Binds.Character.MainShoot.canceled += OnAttackInputCanceled;
        }
        else
        {
            PlayerCharacter.Instance.Binds.Character.MainShoot.started -= OnAttackInputReceived;
            PlayerCharacter.Instance.Binds.Character.MainShoot.canceled -= OnAttackInputCanceled;
        }
    }

    private void OnAttackInputReceived(InputAction.CallbackContext obj)
    {
        _playerAnimator.ThrowAnim();
        PlayerCharacter.Instance.ServerSpawnObject(_tntThrowablePrefab, PlayerCharacter.Instance.DropPoint.position, PlayerCharacter.Instance.CameraTransform.rotation);
    }
    
    private void OnAttackInputCanceled(InputAction.CallbackContext obj)
    {
        _playerAnimator.DisableAllBools();
    }

    private void OnDisable()
    {
        _inventoryItem.ChangeEquipState -= OnChangeEquipState;
        PlayerCharacter.Instance.Binds.Character.MainShoot.started -= OnAttackInputReceived;
        PlayerCharacter.Instance.Binds.Character.MainShoot.canceled -= OnAttackInputCanceled;
    }
}
