using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EquipItem : MonoBehaviour
{ 
    [field :SerializeField] public PlayerAnimator PlayerAnimator { get; private set; }
    [SerializeField] private ItemAnimatorEventHandler _animatorEventHandler;
    [SerializeField] private ItemAnimator _animator;
    [SerializeField] private PlayerInventoryItem _inventoryItem;

    private void OnEnable()
    {
        _inventoryItem.ChangeEquipState += OnChangeEquipState;
    }

    private void OnChangeEquipState(bool equiped)
    {
        if (equiped)
        {
            _animatorEventHandler.ChooseItemAnimator(_animator);
            PlayerCharacter.Instance.Binds.Character.MainShoot.started += OnAttackInputReceived;
            PlayerCharacter.Instance.Binds.Character.MainShoot.canceled += OnAttackInputCanceled;
        }
        else
        {
            PlayerAnimator.DisableAllBools();
            PlayerCharacter.Instance.Binds.Character.MainShoot.started -= OnAttackInputReceived;
            PlayerCharacter.Instance.Binds.Character.MainShoot.canceled -= OnAttackInputCanceled;
        }
    }

    public abstract void OnAttackInputReceived(InputAction.CallbackContext obj);
    
    public virtual void OnAttackInputCanceled(InputAction.CallbackContext obj)
    {
    //    PlayerAnimator.DisableAllBools();
    }

    private void OnDisable()
    {
        _inventoryItem.ChangeEquipState -= OnChangeEquipState;
        PlayerCharacter.Instance.Binds.Character.MainShoot.started -= OnAttackInputReceived;
        PlayerCharacter.Instance.Binds.Character.MainShoot.canceled -= OnAttackInputCanceled;
    }
}
