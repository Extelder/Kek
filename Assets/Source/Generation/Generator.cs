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
    [SerializeField] private bool _notDeleate;

    [SerializeField] private Transform[] _oreSpawnPoint;

    [field: ShowIf(nameof(IsInstance)), SerializeField]
    private GameObject[] _defaultSpawningObjects;

    [SerializeField] private GameObject _block;

    [SerializeField] private GameObject[] _spawnableParts;

    public List<GameObject> SpawningParts { get; private set; } = new List<GameObject>();

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


    [field: ShowIf(nameof(IsInstance)), SerializeField]
    public List<Transform> OreSpawnPlaces { get; private set; } = new List<Transform>();

    public static Generator Instance { get; private set; }

    [field: SerializeField] public bool IsInstance { get; private set; }

    public static event Action GenerationEnd;

    public event Action Regenerate;

    public event Action GenerateStarted;

    private int _spawnedEnemies;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsServer)
        {
            if (IsInstance)
            {
                Instance = this;
                SpawningParts = new List<GameObject>(_defaultSpawningObjects.Length);
                for (int i = 0; i < _defaultSpawningObjects.Length; i++)
                {
                    SpawningParts.Add(_defaultSpawningObjects[i]);
                }
            }
            else
            {
                foreach (var transform1 in _oreSpawnPoint)
                {
                    if (transform1 == null)
                        continue;
                    Instance.OreSpawnPlaces.Add(transform1);
                }
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
        Instance.OreSpawnPlaces?.Clear();
        Regenerate?.Invoke();
        Generatable[] objects =
            GameObject.FindObjectsByType<Generatable>(FindObjectsSortMode.None);

        foreach (var i in objects)
        {
            if (i != Instance.GetComponent<Generatable>())
                i.Despawn();
        }

        Instance.SpawningParts = new List<GameObject>(Instance._defaultSpawningObjects.Length);
        for (int k = 0; k < Instance._defaultSpawningObjects.Length; k++)
        {
            Instance.SpawningParts.Add(Instance._defaultSpawningObjects[k]);
        }

        _spawnedEnemies = 0;

        Instance.SpawnedEnemySpawnPoint.Clear();
        Instance.SpawnedGenerateParts = 0;
        Instance.GenerateMulticast();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_notDeleate)
            return;
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
        GenerateStarted?.Invoke();
        PlayerCharacter character = PlayerCharacter.Instance;

        for (int i = 0; i < _enemySpawnPoint.Length; i++)
        {
            if (_enemySpawnPoint[0] == null)
                continue;
            Instance.SpawnedEnemySpawnPoint.Add(_enemySpawnPoint[0]);
        }

        for (int i = 0; i < _spawnNextPoints.Length; i++)
        {
            if (IsInstance)
            {
                GameObject forward = _spawnableParts[0];

                character.ServerSpawnObject(forward, _spawnNextPoints[i].position,
                    Quaternion.LookRotation(_spawnNextPoints[i].forward));
                continue;
            }

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

            if (Instance.SpawningParts.Count == 0)
            {
                Instance.SpawningParts = new List<GameObject>(Instance._defaultSpawningObjects.Length);
                for (int k = 0; k < Instance._defaultSpawningObjects.Length; k++)
                {
                    Instance.SpawningParts.Add(Instance._defaultSpawningObjects[k]);
                }
            }

            if (Instance.SpawnedGenerateParts % 2 == 0)
            {
                GameObject forward = Instance._spawnableParts[1];

                character.ServerSpawnObject(forward, _spawnNextPoints[i].position,
                    Quaternion.LookRotation(_spawnNextPoints[i].forward));
                continue;
            }


            GameObject objectToSpawn = Instance.SpawningParts[Random.Range(0, Instance.SpawningParts.Count - 1)];

            character.ServerSpawnObject(objectToSpawn, _spawnNextPoints[i].position,
                Quaternion.LookRotation(_spawnNextPoints[i].forward));

            Instance.SpawningParts.Remove(objectToSpawn);
        }
    }

    private void OnDisable()
    {
        GenerationEnd -= OnGenerationEnd;
    }
}