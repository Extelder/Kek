using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class SmallGenerator : NetworkBehaviour
{
    [SerializeField] private Generator _generator;

    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private GameObject _block;
    [SerializeField] private GameObject _firstSpawn;

    [SerializeField] private int _maxToSpawn;


    private int _currentSpawnedObjects;

    [SerializeField] private SmallGeneratable[] _generatables;

    public override void OnStartClient()
    {
        if (!base.IsServer)
            return;
        _generator = Generator.Instance;
        SmallGeneratable.Spawned += OnSmallGeneratableSpawned;
        _generator.Regenerate += OnRegenerate;
        PlayerCharacter.Instance.ServerSpawnObject(
            _firstSpawn,
            _spawnPoint.position,
            Quaternion.LookRotation(_spawnPoint.forward));
    }

    private void OnRegenerate()
    {
        if (!base.IsServer)
            return;
        _currentSpawnedObjects = 0;
        PlayerCharacter.Instance.ServerSpawnObject(
            _firstSpawn,
            _spawnPoint.position,
            Quaternion.LookRotation(_spawnPoint.forward));
    }

    private void OnSmallGeneratableSpawned(SmallGeneratable generatable)
    {
        for (int i = 0; i < generatable.SpawnPoints.Length; i++)
        {
            if (_currentSpawnedObjects > _maxToSpawn)
            {
                PlayerCharacter.Instance.ServerSpawnObject(_block, generatable.SpawnPoints[i].position,
                    Quaternion.identity);
                continue;
            }

            if (_currentSpawnedObjects % 2 == 0)
            {
                PlayerCharacter.Instance.ServerSpawnObject(
                    _generatables[0].gameObject,
                    generatable.SpawnPoints[i].position,
                    Quaternion.LookRotation(generatable.SpawnPoints[i].forward));
            }
            _currentSpawnedObjects++;
            PlayerCharacter.Instance.ServerSpawnObject(
                _generatables[Random.Range(0, _generatables.Length)].gameObject,
                generatable.SpawnPoints[i].position,
                Quaternion.LookRotation(generatable.SpawnPoints[i].forward));
        }
    }

    private void OnDisable()
    {
        if (!base.IsServer)
            return;
        SmallGeneratable.Spawned -= OnSmallGeneratableSpawned;
        _generator.Regenerate -= OnRegenerate;
    }
}