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
        _inventoryItem.ItemEnabled += OnItemEnabled;
        _inventoryItem.ItemDisabled += OnItemDisabled;
    }

    private void OnItemDisabled()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void OnItemEnabled()
    {
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    private void OnItemDataChanged(ItemData itemData)
    {
        if (itemData == null)
        {
            _itemImage.sprite = null;
            return;
        }

        _itemImage.sprite = itemData.Icon;
    }

    private void OnDisable()
    {
        _inventoryItem.ItemDataChanged -= OnItemDataChanged;
        _inventoryItem.ItemEnabled -= OnItemEnabled;
        _inventoryItem.ItemDisabled -= OnItemDisabled;
    }
}