using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VendingMachineInteractableChangeText : MonoBehaviour
{
    [SerializeField] private VendingMachineInteractableSpawnable _vendingMachineInteractableSpawnable;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _priceText;
    private void OnEnable()
    {
        _vendingMachineInteractableSpawnable.TextChanged += OnTextChanged;
        _vendingMachineInteractableSpawnable.PriceChanged += OnPriceChanged;
    }

    private void OnTextChanged(string text)
    {
        _nameText.text = text;
    }

    private void OnPriceChanged(int value)
    {
        _priceText.text = value + "$";
    }

    private void OnDisable()
    {
        _vendingMachineInteractableSpawnable.TextChanged -= OnTextChanged;
        _vendingMachineInteractableSpawnable.PriceChanged -= OnPriceChanged;
    }
}
