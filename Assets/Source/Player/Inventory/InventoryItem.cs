using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class InventoryItem : MonoBehaviour
{
    [field: SerializeField] public bool IsEquiped { get; private set; }
    [field: SerializeField] public ItemData CurrentItemData { get; private set; }

    public event Action<ItemData> ItemDataChanged;

    public static event Action<ItemData> ItemEquiped;

    [Button]
    public void Equip()
    {
        if (CurrentItemData != null)
        {
            ItemEquiped?.Invoke(CurrentItemData);
        }
    }

    public void ChangeItemData(ItemData itemData)
    {
        if (CurrentItemData != null)
        {
            PlayerCharacter character = PlayerCharacter.Instance;
            character.ServerSpawnObject(CurrentItemData.Prefab, character.DropPoint.position, Quaternion.identity);
        }

        CurrentItemData = itemData;
        ItemDataChanged?.Invoke(CurrentItemData);
    }
}