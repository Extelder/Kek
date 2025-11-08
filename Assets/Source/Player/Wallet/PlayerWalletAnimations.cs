using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerWalletAnimations : MonoBehaviour
{
    [SerializeField] private PlayerWallet _wallet;
    
    [SerializeField] private Animator _animator;
    [SerializeField] private string _boolName;

    [SerializeField] private float _coolDown;

    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private Color _gainColor;
    [SerializeField] private Color _spendColor;

    private void OnEnable()
    {
        _wallet.MoneyChanged += OnMoneyChanged;
    }

    private void OnMoneyChanged(int value)
    {
        Debug.Log("onmoneyChanged");
        StartCoroutine(RevertChanged());
        if (value >= 0)
        {
            _valueText.color = _gainColor;
            _animator.SetBool(_boolName, true);
            _valueText.text = "+" + value + "$";
            return;
        }
        _valueText.color = _spendColor;
        _animator.SetBool(_boolName, false);
        _valueText.text = value + "$";
    }

    private IEnumerator RevertChanged()
    {
        _valueText.gameObject?.SetActive(false);
        _valueText.gameObject.SetActive(true);
        yield return new WaitForSeconds(_coolDown);
        _valueText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _wallet.MoneyChanged -= OnMoneyChanged;
    }
}
