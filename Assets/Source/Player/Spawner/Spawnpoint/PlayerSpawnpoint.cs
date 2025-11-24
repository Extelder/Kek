using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawnpoint : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;

    private void OnEnable()
    {
        PlayerSpawnable.Spawned += OnPlayerSpawned;
    }

    private void OnPlayerSpawned(GameObject playerObject)
    {
        playerObject.transform.position = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position +
                                          new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
    }

    private void OnDisable()
    {
        PlayerSpawnable.Spawned -= OnPlayerSpawned;
    }
}