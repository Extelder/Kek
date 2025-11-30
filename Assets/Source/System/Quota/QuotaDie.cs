using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuotaDie : MonoBehaviour
{
    [SerializeField] private GameObject[] _lights;

    [SerializeField] private float _deathDelay;

    [SerializeField] private Quota _quota;

    private void OnEnable()
    {
        _quota.ValueChanged += OnQuotaValueChanged;
    }

    private void OnQuotaValueChanged(float value)
    {
        if (value > 0)
            return;

        for (int i = 0; i < _lights.Length; i++)
        {
            if (_lights[i] != null)
                _lights[i].SetActive(false);
        }

        StartCoroutine(WaitingForDeath());
    }

    private IEnumerator WaitingForDeath()
    {
        yield return new WaitForSeconds(_deathDelay);
        PlayerCharacter.Instance.PlayerHealth.TakeDamage(10000000f);
    }

    private void OnDisable()
    {
        _quota.ValueChanged -= OnQuotaValueChanged;
    }
}