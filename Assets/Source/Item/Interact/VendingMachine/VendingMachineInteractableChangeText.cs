using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachineInteractableChangeText : MonoBehaviour
{
    [SerializeField] private VendingMachineInteractableSpawnable _vendingMachineInteractableSpawnable;

    [SerializeField] private Image _image;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _priceText;

    private void OnEnable()
    {
        _vendingMachineInteractableSpawnable.DataChanged += OnTextChanged;
    }

    private void OnValidate()
    {
        _nameText.text = _vendingMachineInteractableSpawnable.ItemData.Name;
        _priceText.text = _vendingMachineInteractableSpawnable.ItemData.Price + "$";
        _image.sprite = _vendingMachineInteractableSpawnable.ItemData.Image;
    }

    private void OnTextChanged(string text, int value, Sprite sprite)
    {
        _nameText.text = text;
        _priceText.text = value + "$";
        _image.sprite = sprite;
    }

    private void OnDisable()
    {
        _vendingMachineInteractableSpawnable.DataChanged -= OnTextChanged;
    }
}