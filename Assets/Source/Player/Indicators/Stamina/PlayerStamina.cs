using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStamina : Stamina
{
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerCharacter _character;

    [SerializeField] private float _spendRate;
    [SerializeField] private float _recoverRate;

    [SerializeField] private float _timeToRecover;

    [SerializeField] private float _valueToSpend;
    [SerializeField] private float _valueToRecover;

    private CompositeDisposable _spendDisposable = new CompositeDisposable();
    private CompositeDisposable _recoverDisposable = new CompositeDisposable();

    private void OnEnable()
    {
        _character.ClientStarted += OnClientStarted;
    }

    private void OnClientStarted()
    {
        if (!_character.IsOwner)
            return;
        _character.Binds.Character.Run.started += OnPlayerStartedMoving;
        _character.Binds.Character.Run.canceled += OnPlayerStoppedMoving;
    }

    private void OnPlayerStartedMoving(InputAction.CallbackContext obj)
    {
        StopAllCoroutines();
        _recoverDisposable.Clear();
        Observable.Interval(TimeSpan.FromSeconds(_spendRate)).Subscribe(_ => { Spend(_valueToSpend); })
            .AddTo(_spendDisposable);
    }

    private IEnumerator RecoverStamina()
    {
        yield return new WaitForSeconds(_timeToRecover);
        Observable.Interval(TimeSpan.FromSeconds(_recoverRate)).Subscribe(_ =>
        {
            _movement.CanRun = true;
            Add(_valueToRecover);
        }).AddTo(_recoverDisposable);
    }


    private void OnPlayerStoppedMoving(InputAction.CallbackContext obj)
    {
        _spendDisposable.Clear();
        StartCoroutine(RecoverStamina());
    }

    public override void Lost()
    {
        CurrentValue = 0;
        _movement.StopRun();
        _movement.CanRun = false;

        StaminaValueChanged?.Invoke(CurrentValue);
    }

    private void OnDisable()
    {
        if (!base.IsOwner)
            return;
        _character.ClientStarted -= OnClientStarted;
        _character.Binds.Character.Run.started -= OnPlayerStartedMoving;
        _character.Binds.Character.Run.canceled -= OnPlayerStoppedMoving;
        _spendDisposable.Clear();
        _recoverDisposable.Clear();
    }
}