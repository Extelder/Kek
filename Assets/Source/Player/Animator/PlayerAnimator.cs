using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private string _attack;

    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _fpsAnimator;
    [SerializeField] private float _speed;

    [SerializeField] private PlayerCharacter _character;

    private PlayerBinds _binds;

    private Vector3 _inputVector;
    private Vector3 _finalVector;

    private CompositeDisposable _disposable = new CompositeDisposable();

    private void OnEnable()
    {
        _character.ClientStarted += OnClienStarted;
    }

    private void OnClienStarted()
    {
        if (!_character.IsOwner)
            return;

        _binds = _character.Binds;

        Observable.EveryUpdate().Subscribe(_ =>
        {
            _inputVector = new Vector3(_binds.Character.Horizontal.ReadValue<float>(), 0,
                _binds.Character.Vertical.ReadValue<float>());

            _finalVector.x = Mathf.Lerp(_finalVector.x, _inputVector.x, _speed * Time.deltaTime);
            _finalVector.z = Mathf.Lerp(_finalVector.z, _inputVector.z, _speed * Time.deltaTime);

            _animator.SetFloat("XVelocity", _finalVector.x);
            _animator.SetFloat("YVelocity", _finalVector.z);
        }).AddTo(_disposable);
    }

    public void AttackAnim()
    {
        SetAnimationBoolAndDisableOthers(_attack, true);
    }

    public void SetAnimationBoolAndDisableOthers(string name, bool value)
    {
        _animator.SetBool(name, value);
        _fpsAnimator.SetBool(name, value);
        DisableAllBools();
    }

    public void SetAnimationBool(string name, bool value)
    {
        _animator.SetBool(name, value);
        _fpsAnimator.SetBool(name, value);
    }

    public void DisableAllBools()
    {
        SetAnimationBool(_attack, false);
    }

    private void OnDestroy()
    {
        _character.ClientStarted -= OnClienStarted;
        _disposable?.Clear();
    }
}