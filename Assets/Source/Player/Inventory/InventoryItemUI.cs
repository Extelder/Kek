using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image _itemImage;

    [SerializeField] private InventoryItem _inventoryItem;

    private void OnEnable()
    {
        _inventoryItem.ItemDataChanged += OnItemDataChanged;
    }

    private void OnItemDataChanged(ItemData itemData)
    {
        _itemImage.sprite = itemData.Icon;
    }

    private void OnDisable()
    {
        _inventoryItem.ItemDataChanged -= OnItemDataChanged;
    }
}