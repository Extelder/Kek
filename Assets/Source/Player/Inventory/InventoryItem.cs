using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

public class InventoryItem : NetworkBehaviour
{
    [field: SerializeField] public bool IsEquiped { get; private set; }
    [field: SerializeField] public ItemData CurrentItemData { get; private set; }

    [SerializeField] private string _actionName;

    public event Action<ItemData> ItemDataChanged;

    public static event Action<ItemData> ItemEquiped;

    public static InventoryItem CurrentInventoryItem;

    public event Action ItemEnabled;
    public event Action ItemDisabled;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            PlayerCharacter.Instance.Binds.FindAction(_actionName, true).started += OnEquipActionPerformed;
            PlayerCharacter.Instance.Binds.Character.Drop.started += OnTryToDrop;
            InventoryItem.ItemEquiped += OnSomeItemEquiped;
        }
    }

    private void OnTryToDrop(InputAction.CallbackContext obj)
    {
        if (!IsEquiped)
            return;
        if (CurrentItemData != null)
        {
            PlayerCharacter character = PlayerCharacter.Instance;
            character.ServerSpawnObject(CurrentItemData.Prefab, character.DropPoint.position, Quaternion.identity);
            CurrentItemData = null;
            ItemDataChanged?.Invoke(CurrentItemData);
            Equip();
        }
    }

    private void OnSomeItemEquiped(ItemData obj)
    {
        if (CurrentInventoryItem == this)
        {
            ItemEnabled?.Invoke();
        }
        else
        {
            ItemDisabled?.Invoke();
            IsEquiped = false;
        }
    }


    private void OnEquipActionPerformed(InputAction.CallbackContext obj)
    {
        Equip();
    }

    [Button]
    public void Equip()
    {
        CurrentInventoryItem = this;
        IsEquiped = true;
        ItemEquiped?.Invoke(CurrentItemData);
    }

    public void ChangeItemData(ItemData itemData)
    {
        if (CurrentItemData != null)
        {
            PlayerCharacter character = PlayerCharacter.Instance;
            character.ServerSpawnObject(CurrentItemData.Prefab, character.DropPoint.position, Quaternion.identity);
            CurrentItemData = null;
        }

        CurrentItemData = itemData;
        ItemDataChanged?.Invoke(CurrentItemData);
        Equip();
    }

    private void OnDisable()
    {
        CurrentInventoryItem = null;
        if (base.IsOwner)
        {
            PlayerCharacter.Instance.Binds.FindAction(_actionName, true).started -= OnEquipActionPerformed;
            PlayerCharacter.Instance.Binds.Character.Drop.started -= OnTryToDrop;
            InventoryItem.ItemEquiped -= OnSomeItemEquiped;
        }
    }
}