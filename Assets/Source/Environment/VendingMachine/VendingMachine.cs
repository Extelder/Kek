using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    [SerializeField] private Transform _spawnOrigin;
    [SerializeField] private NetWorkAnimatorSynchronize _animatorSynchronize;
    [SerializeField] private string _boolName;
    [SerializeField] private float _coolDown;

    [field: SerializeField] public bool CanInteract { get; private set; } = true;
    private BuyableItemData _currentBuyable;
    
    public void GetCurrentBuyable(BuyableItemData buyableItemData)
    {
        Debug.Log("OnBought");
        _currentBuyable = buyableItemData;
        _animatorSynchronize.SetAnimatorBoolMulticast(_boolName, true);
        CanInteract = false;
    }
    
    public void SpawnItem()
    {
        PlayerCharacter.Instance.ServerSpawnObject(_currentBuyable.Prefab, _spawnOrigin.position, Quaternion.identity);
        StartCoroutine(DisableBool());
    }

    private IEnumerator DisableBool()
    {
        yield return new WaitForSeconds(_coolDown);
        _animatorSynchronize.SetAnimatorBoolMulticast(_boolName, false);
        CanInteract = true;
    }
}