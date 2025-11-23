using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageIndicator : MonoBehaviour
{
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private GameObject _damageIndicator;
    [SerializeField] private float _showRate;

    private void OnEnable()
    {
        _health.Damaged += OnPlayerDamaged;
    }

    private void OnPlayerDamaged(float value)
    {
        StopAllCoroutines();
        StartCoroutine(ShowIndicator());
    }

    private IEnumerator ShowIndicator()
    {
        _damageIndicator.SetActive(true);
        yield return new WaitForSeconds(_showRate);
        _damageIndicator.SetActive(false);
    }

    private void OnDisable()
    {
        _health.Damaged += OnPlayerDamaged;
    }
}
