using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStamina : Stamina
{
    [SerializeField] private PlayerCharacter _character;
    [SerializeField] private float _spendRate; 
    [SerializeField] private float _valueToSpend; 
    private CompositeDisposable _spendDisposable = new CompositeDisposable();
    
    private void OnEnable()
    {
        _character.ClientStarted += OnClientStarted;
    }

    private void OnClientStarted()
    {
        if (!_character.IsOwner)
            return;
        _character.Binds.Character.Run.started += OnPlayerMoving;
        _character.Binds.Character.Run.canceled += OnPlayerStoppedMoving;
    }

    private void OnPlayerMoving(InputAction.CallbackContext obj)
    {
        Debug.Log("MOVING");
        Observable.Interval(TimeSpan.FromSeconds(_spendRate)).Subscribe(_ =>
        {
            Spend(_valueToSpend);
        }).AddTo(_spendDisposable);
    }

    private void OnPlayerStoppedMoving(InputAction.CallbackContext obj)
    {
        Debug.Log("STOPPEDMOVING");
        _spendDisposable.Clear();
    }

    public override void Lost()
    {
        CurrentValue = 0;
        StaminaValueChanged?.Invoke(CurrentValue);
        Debug.Log("NoStamina");
    }

    private void OnDisable()
    {
        if (!base.IsOwner)
            return;
        _character.ClientStarted -= OnClientStarted;
        _character.Binds.Character.Run.started -= OnPlayerMoving;
        _character.Binds.Character.Run.canceled -= OnPlayerStoppedMoving;
        _spendDisposable.Clear();
    }
}
