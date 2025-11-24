using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnItemOnPlaceDialogueEvent : DialogueEvent
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _spawnPoint;

    public override void Invoke()
    {
        PlayerCharacter.Instance.ServerSpawnObject(_prefab, _spawnPoint.position, Quaternion.identity);
    }
}