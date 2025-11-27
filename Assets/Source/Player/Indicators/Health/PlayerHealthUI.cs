using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealthUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private PlayerHealth _health;

    private void OnEnable()
    {
        _health.HealthValueChanged += OnHealthValueChanged;
    }

    public override void OnStartClient()
    {
        if (!base.IsOwner)
        {
            _health.HealthValueChanged -= OnHealthValueChanged;
            _healthText.enabled = false;
            return;
        }

        OnHealthValueChanged(_health.GetCurrentValue());
    }

    private void OnHealthValueChanged(float value)
    {
        _healthText.text = $"{value}";

        float t = Mathf.Clamp01(value / _health.MaxValue);

        Color color = Color.Lerp(Color.red, Color.white, t);
        _healthText.color = color;
    }

    private void OnDisable()
    {
        _health.HealthValueChanged -= OnHealthValueChanged;
    }
}