using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerInventoryItem : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private GameObject _thirdPersonGameObject;
    [SerializeField] private ItemData _needItem;

    private void OnEnable()
    {
        InventoryItem.ItemEquiped += OnItemEquiped;
    }

    private void Start()
    {
        _thirdPersonGameObject.SetActive(false);
    }

    private void OnItemEquiped(ItemData data)
    {
        _gameObject.SetActive(data == _needItem);
        Debug.Log(PlayerCharacter.Instance);
        PlayerCharacter.Instance.SetObjectEnableServer(_thirdPersonGameObject, (data == _needItem));
    }


    private void OnDisable()
    {
        InventoryItem.ItemEquiped -= OnItemEquiped;
    }
}