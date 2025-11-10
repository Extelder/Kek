using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OreDestroyed : MonoBehaviour
{
    [SerializeField] private Ore _ore;
    [SerializeField] private GameObject _spawnableGameObject;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _minCountToSpawn;
    [SerializeField] private int _maxCountToSpawn;
    
    private int _countToSpawn;

    private void OnEnable()
    {
        _ore.Destroyed += OnDestroyed;
    }

    private void OnDestroyed()
    {
        _countToSpawn = Random.Range(_minCountToSpawn, _maxCountToSpawn);
        for (int i = 0; i < _countToSpawn; i++)
        {
            PlayerCharacter.Instance.ServerSpawnObject(_spawnableGameObject, _spawnPoint.position, Quaternion.identity);
        }
    }

    private void OnDisable()
    {
        _ore.Destroyed -= OnDestroyed;
    }
}
