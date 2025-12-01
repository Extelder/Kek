using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class PlayerStaminaUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _staminaText;
    [SerializeField] private PlayerStamina _stamina;

    private void OnEnable()
    {
        _stamina.StaminaValueChanged += OnStaminaValueChanged;
    }

    public override void OnStartClient()
    {
        if (!base.IsOwner)
        {
            _stamina.StaminaValueChanged -= OnStaminaValueChanged;
            _staminaText.enabled = false;
            return;
        }

        OnStaminaValueChanged(_stamina.GetCurrentValue());
    }

    private void OnStaminaValueChanged(float value)
    {
        _staminaText.text = $"{value}";

        float t = Mathf.Clamp01(value / _stamina.MaxValue);

        Color color = Color.Lerp(Color.red, Color.white, t);

        _staminaText.color = color;
    }

    private void OnDisable()
    {
        _stamina.StaminaValueChanged -= OnStaminaValueChanged;
    }
}