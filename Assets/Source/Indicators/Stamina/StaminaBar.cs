using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image _staminaBar;
    [SerializeField] private Stamina _stamina;

    protected void OverrideStamina(Stamina stamina)
    {
        _stamina = stamina;
    }

    private void OnEnable()
    {
        _stamina.StaminaValueChanged += OnStaminaValueChanged;
    }

    private void OnDisable()
    {
        _stamina.StaminaValueChanged -= OnStaminaValueChanged;
    }

    public virtual void OnStaminaValueChanged(float value)
    {
        float percent = _stamina.MaxValue / 100;
        _staminaBar.fillAmount = value / percent / 100;
    }
}
