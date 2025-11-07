using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEventsSpawner : NetworkBehaviour
{
    [SerializeField] private Generator _generator;
    [SerializeField] private RandomEvent[] _events;

    [SerializeField] private Vector2Int _randomEventsToSpawnRange;

    private List<RandomEvent> SpawningEvents = new List<RandomEvent>();

    private List<NetworkObject> SpawnedEvents = new List<NetworkObject>();

    public static RandomEventsSpawner Instance { get; private set; }

    public override void OnStartClient()
    {
        if (!base.IsServer)
            return;
        Instance = this;

        _generator.Regenerate += OnRegenerate;
        SpawningEvents = new List<RandomEvent>(_events.Length);
        for (int i = 0; i < _events.Length; i++)
        {
            SpawningEvents.Add(_events[i]);
        }

        int toSpawnNumber = Random.Range(_randomEventsToSpawnRange.x, _randomEventsToSpawnRange.y);

        for (int i = 0; i < toSpawnNumber; i++)
        {
            RandomEvent eventToSpawn = SpawningEvents[Random.Range(0, SpawningEvents.Count - 1)];

            PlayerCharacter.Instance.ServerSpawnObject(eventToSpawn.gameObject, transform.position,
                Quaternion.identity);

            SpawningEvents.Remove(eventToSpawn);
        }
    }


    public void RegisterSpawnedEvent(NetworkObject spawnedEvent)
    {
        if (!base.IsServer)
            return;
        SpawnedEvents.Add(spawnedEvent);
    }

    private void OnDisable()
    {
        if (!base.IsServer)
            return;
        _generator.Regenerate -= OnRegenerate;
    }

    private void OnRegenerate()
    {
        if (!base.IsServer)
            return;

        for (int i = 0; i < SpawnedEvents.Count; i++)
        {
            SpawnedEvents[i].Despawn();
        }

        SpawnedEvents?.Clear();

        SpawningEvents = new List<RandomEvent>(_events.Length);
        for (int i = 0; i < _events.Length; i++)
        {
            SpawningEvents.Add(_events[i]);
        }

        int toSpawnNumber = Random.Range(_randomEventsToSpawnRange.x, _randomEventsToSpawnRange.y);

        for (int i = 0; i < toSpawnNumber; i++)
        {
            RandomEvent eventToSpawn = SpawningEvents[Random.Range(0, SpawningEvents.Count - 1)];

            PlayerCharacter.Instance.ServerSpawnObject(eventToSpawn.gameObject, transform.position,
                Quaternion.identity);

            SpawningEvents.Remove(eventToSpawn);
        }
    }
}