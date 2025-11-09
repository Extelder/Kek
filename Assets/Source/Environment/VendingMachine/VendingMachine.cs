using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UniRx;
using UnityEngine;

public class VendingMachine : NetworkBehaviour
{
    [SerializeField] private Transform _spawnOrigin;
    [SerializeField] private NetWorkAnimatorSynchronize _animatorSynchronize;
    [SerializeField] private string _boolName;
    [SerializeField] private float _coolDown;

    [field: SerializeField] public bool CanBuy { get; private set; } = true;
    [field: SerializeField] public bool CanInteract = true;
    private BuyableItemData _currentBuyable;

    public override void OnStartServer()
    {
        base.OnStartServer();
        NotifyValueChanged(CanInteract);
    }

    public void GetCurrentBuyable(BuyableItemData buyableItemData)
    {
        Debug.Log("OnBought");
        _currentBuyable = buyableItemData;
        _animatorSynchronize.SetAnimatorBoolMulticast(_boolName, true);
        CanBuy = false;
    }
    
    public void SetInteractBool(bool value)
    {
        Set(value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void Set(bool value)
    {
        if (!IsServer)
            return;

        CanInteract = value;
        NotifyValueChanged(CanInteract);
        Debug.Log(CanInteract + "Server");
    }
    
    [ObserversRpc(BufferLast = true)]
    private void NotifyValueChanged(bool value)
    {
        CanInteract = value;
        Debug.Log(CanInteract + "Observer");
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
        CanBuy = true;
    }
}