using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EquipItem : MonoBehaviour
{
    [field: SerializeField] public PlayerAnimator PlayerAnimator { get; private set; }
    [SerializeField] private ItemAnimatorEventHandler _animatorEventHandler;
    [SerializeField] private bool _changeCameraTransform;
    [ShowIf(nameof(_changeCameraTransform)), SerializeField] private Transform _cameraTransform;
    [SerializeField] private ItemAnimator _animator;
    [SerializeField] private PlayerInventoryItem _playerInventoryItem;
    [SerializeField] private string _actionName;

    private Vector3 _defaultRotation;
    protected bool _equiped;

    private void OnEnable()
    {
        _playerInventoryItem.ChangeEquipState += OnChangeEquipState;
        _playerInventoryItem.EquipmentNull += OnEquipmentNull;
        if (_changeCameraTransform)
            _defaultRotation = _cameraTransform.localEulerAngles;
    }

    public virtual void OnEquipmentNull()
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
            OnUnEquiped();
        }
    }

    public virtual void OnEquipStateChanged()
    {
    }
    
    public virtual void OnUnEquiped()
    {
    }

    public void SetCameraTransformValue(Vector3 rotationValue)
    {
        _cameraTransform.localEulerAngles = rotationValue;
    }

    public void SetCameraTransformDefaultValue()
    {
        _cameraTransform.localEulerAngles = _defaultRotation;
    }

    public abstract void OnInputReceived(InputAction.CallbackContext obj);

    public abstract void OnInputCanceled(InputAction.CallbackContext obj);

    private void OnDisable()
    {
        _playerInventoryItem.ChangeEquipState -= OnChangeEquipState;
        _playerInventoryItem.EquipmentNull -= OnEquipmentNull;
        PlayerCharacter.Instance.Binds.FindAction(_actionName, true).started -= OnInputReceived;
        PlayerCharacter.Instance.Binds.FindAction(_actionName, true).canceled -= OnInputCanceled;
        if (_changeCameraTransform)
            SetCameraTransformDefaultValue();
        OnDisableVirtual();
    }
    
    public virtual void OnDisableVirtual(){}
}