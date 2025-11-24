using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageIndicator : MonoBehaviour
{
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private Animator _damageIndicatorAnimator;
    [SerializeField] private Animator _damageIndicatorAnimator2;
    [SerializeField] private float _showRate;

    private void OnEnable()
    {
        _health.Damaged += OnPlayerDamaged;
    }

    private void OnPlayerDamaged(float value)
    {
        ShowIndicator();
    }

    private void ShowIndicator()
    {
        _damageIndicatorAnimator.Play("Blood", 0, 0f);
        _damageIndicatorAnimator2.Play("Blood1", 0, 0f);
    }

    private void OnDisable()
    {
        _health.Damaged -= OnPlayerDamaged;
    }
}
