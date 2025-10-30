using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EquipItem : MonoBehaviour
{
    [field: SerializeField] public PlayerAnimator PlayerAnimator { get; private set; }
    [SerializeField] private ItemAnimatorEventHandler _animatorEventHandler;
    [SerializeField] private ItemAnimator _animator;
    [SerializeField] private PlayerInventoryItem _playerInventoryItem;
    [SerializeField] private string _actionName;

    protected bool _equiped;

    private void OnEnable()
    {
        _playerInventoryItem.ChangeEquipState += OnChangeEquipState;
        _playerInventoryItem.EquipmentNull += OnEquipmentNull;
    }

    private void OnEquipmentNull()
    {
        _animatorEventHandler.ChooseItemAnimator(null);
    }

    private void OnChangeEquipState(bool equiped)
    {
        _equiped = equiped;
        if (equiped)
        {
            _animatorEventHandler.ChooseItemAnimator(_animator);
            PlayerCharacter.Instance.Binds.FindAction(_actionName, true).started += OnInputReceived;
            PlayerCharacter.Instance.Binds.FindAction(_actionName, true).canceled += OnInputCanceled;
            OnEquipStateChanged();
        }
        else
        {
            PlayerAnimator.DisableAll();
            PlayerCharacter.Instance.Binds.FindAction(_actionName, true).started -= OnInputReceived;
            PlayerCharacter.Instance.Binds.FindAction(_actionName, true).canceled -= OnInputCanceled;
        }
    }

    public virtual void OnEquipStateChanged()
    {
    }

    public abstract void OnInputReceived(InputAction.CallbackContext obj);

    public abstract void OnInputCanceled(InputAction.CallbackContext obj);

    private void OnDisable()
    {
        _playerInventoryItem.ChangeEquipState -= OnChangeEquipState;
        _playerInventoryItem.EquipmentNull -= OnEquipmentNull;
        PlayerCharacter.Instance.Binds.FindAction(_actionName, true).started -= OnInputReceived;
        PlayerCharacter.Instance.Binds.FindAction(_actionName, true).canceled -= OnInputCanceled;
    }
}