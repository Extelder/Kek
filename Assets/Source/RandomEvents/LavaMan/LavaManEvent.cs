using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using UnityEngine;

public class LavaManEvent : RandomEvent
{
    [SerializeField] private GameObject _spawnObject;
    
    public override void StartEvent()
    {
        Transform spawnPoint =
            Generator.Instance.SpawnedEnemySpawnPoint[Random.Range(0, Generator.Instance.SpawnedEnemySpawnPoint.Count)];
        PlayerCharacter.Instance.ServerSpawnObject(_spawnObject, spawnPoint.position, Quaternion.identity);
    }
}
