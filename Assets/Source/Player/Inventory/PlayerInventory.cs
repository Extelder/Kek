using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private InventoryItem[] _inventoryItems;

    public bool TryAddItem(ItemData data)
    {
        for (int i = 0; i < _inventoryItems.Length; i++)
        {
            if (_inventoryItems[i].CurrentItemData == null)
            {
                _inventoryItems[i].ChangeItemData(data);
                return true;
            }
        }

        for (int i = 0; i < _inventoryItems.Length; i++)
        {
            if (_inventoryItems[i].IsEquiped == true)
            {
                _inventoryItems[i].ChangeItemData(data);
                return true;
            }
        }

        return false;
    }
}