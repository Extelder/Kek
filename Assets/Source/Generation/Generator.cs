using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using NaughtyAttributes;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : NetworkBehaviour
{
    [SerializeField] private GameObject _block;

    [SerializeField] private GameObject[] _spawnableParts;

    [SerializeField] private Transform[] _spawnNextPoints;

    [SerializeField] private Transform[] _enemySpawnPoint;

    [field: ShowIf(nameof(IsInstance)), SerializeField]
    public int MaxGenerateParts { get; private set; }

    public List<Transform> SpawnedEnemySpawnPoint { get; private set; } = new List<Transform>();

    [field: ShowIf(nameof(IsInstance)), SerializeField]
    public int SpawnedGenerateParts { get; private set; }

    [field: ShowIf(nameof(IsInstance)), SerializeField]
    public NavMeshSurface Surface { get; private set; }

    [field: ShowIf(nameof(IsInstance)), SerializeField]
    public GameObject[] Enemies { get; private set; }

    [field: ShowIf(nameof(IsInstance)), SerializeField]
    public float RandomChanceToSpawnEnemy { get; private set; }

    [field: ShowIf(nameof(IsInstance))] [SerializeField]
    private int _maxEnemyToSpawn;

    public static Generator Instance { get; private set; }

    [field: SerializeField] public bool IsInstance { get; private set; }

    public static event Action GenerationEnd;

    private int _spawnedEnemies;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsServer)
        {
            if (IsInstance)
            {
                Instance = this;
            }

            Generate();
            GenerationEnd += OnGenerationEnd;
        }
    }

    private void OnGenerationEnd()
    {
        if (IsInstance)
        {
            int enemiesToSpawned = Random.Range(1, _maxEnemyToSpawn);

            for (int i = 0; i < SpawnedEnemySpawnPoint.Count; i++)
            {
                if (SpawnedEnemySpawnPoint[0] == null)
                    continue;
                if (_spawnedEnemies == 0)
                {
                    PlayerCharacter.Instance.ServerSpawnObject(Enemies[Random.Range(0, Enemies.Length)],
                        SpawnedEnemySpawnPoint[i].position, Quaternion.identity);
                    _spawnedEnemies++;
                }
                else if (Random.value < RandomChanceToSpawnEnemy)
                {
                    if (_spawnedEnemies >= enemiesToSpawned)
                        continue;
                    PlayerCharacter.Instance.ServerSpawnObject(Enemies[Random.Range(0, Enemies.Length)],
                        SpawnedEnemySpawnPoint[i].position, Quaternion.identity);
                    _spawnedEnemies++;
                }
            }

            RegenerateNavMeshSurfaceServer();
        }

        DisableCollider();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegenerateNavMeshSurfaceServer()
    {
        RegenerateNavMeshSurfaceObserver();
    }

    [ObserversRpc]
    public void RegenerateNavMeshSurfaceObserver()
    {
        Surface.BuildNavMesh();
    }

    [ServerRpc(RequireOwnership = false)]
    public void Generate()
    {
        GenerateMulticast();
    }


    [ServerRpc(RequireOwnership = false)]
    public void ReGenerate()
    {
        if (!IsServer)
            return;
        Generatable[] objects =
            GameObject.FindObjectsByType<Generatable>(FindObjectsSortMode.None);

        foreach (var i in objects)
        {
            if (i != Instance.GetComponent<Generatable>())
                i.Despawn();
        }

        _spawnedEnemies = 0;

        Instance.SpawnedEnemySpawnPoint.Clear();
        Instance.SpawnedGenerateParts = 0;
        Instance.GenerateMulticast();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsInstance)
            return;
        if (!IsServer)
            return;
        if (other.TryGetComponent<Generator>(out Generator generator))
        {
            PlayerCharacter character = PlayerCharacter.Instance;
            character.ServerSpawnObject(_block, transform.position,
                Quaternion.LookRotation(transform.forward));
            character.DespawnObject(this);
        }
    }

    private void DisableCollider()
    {
        if (IsInstance)
            return;
        GetComponent<Collider>().enabled = false;
    }

    [ObserversRpc]
    public void GenerateMulticast()
    {
        if (!IsServer)
            return;
        PlayerCharacter character = PlayerCharacter.Instance;

        for (int i = 0; i < _enemySpawnPoint.Length; i++)
        {
            if (_enemySpawnPoint[0] == null)
                continue;
            Instance.SpawnedEnemySpawnPoint.Add(_enemySpawnPoint[0]);
        }

        for (int i = 0; i < _spawnNextPoints.Length; i++)
        {
            if (Instance.SpawnedGenerateParts >= Instance.MaxGenerateParts)
            {
                GenerationEnd?.Invoke();

                character.ServerSpawnObject(_block, _spawnNextPoints[i].position,
                    Quaternion.LookRotation(_spawnNextPoints[i].forward));
                continue;
            }

            Instance.SpawnedGenerateParts++;
            if (Instance.SpawnedGenerateParts <= 5 && !IsInstance)
            {
                GetComponent<Collider>().enabled = false;
            }

            GameObject part = (_spawnableParts[Random.Range(0, _spawnableParts.Length)]);

            character.ServerSpawnObject(part, _spawnNextPoints[i].position,
                Quaternion.LookRotation(_spawnNextPoints[i].forward));
        }
    }

    private void OnDisable()
    {
        GenerationEnd -= OnGenerationEnd;
    }
}