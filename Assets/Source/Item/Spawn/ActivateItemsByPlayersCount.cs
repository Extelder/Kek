using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class ActivateItemsByPlayerCount : NetworkBehaviour
{
    [SerializeField] private Transform[] _spawnPositions;
    [SerializeField] private GameObject _objectToSpawn;
    private int _currentPlayerCount;

    public override void OnStartClient()
    {
        base.OnStartClient();
        ServerSetActiveObject();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerSetActiveObject()
    {
        _currentPlayerCount++;
        ObserverSetActiveObject();
    }

    [ObserversRpc(BufferLast = true)]
    public void ObserverSetActiveObject()
    {
        PlayerCharacter.Instance.ServerSpawnObject(_objectToSpawn, _spawnPositions[_currentPlayerCount-1].position,
            _spawnPositions[_currentPlayerCount-1].rotation);
    }
}