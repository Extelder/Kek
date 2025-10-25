using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : NetworkBehaviour
{
    [SerializeField] private GameObject _block;

    [SerializeField] private GameObject[] _spawnableParts;

    [SerializeField] private Transform[] _spawnNextPoints;

    [field: ShowIf(nameof(IsInstance)), SerializeField]
    public int MaxGenerateParts { get; private set; }

    [field: ShowIf(nameof(IsInstance)), SerializeField]
    public int SpawnedGenerateParts { get; private set; }

    public static Generator Instance { get; private set; }

    [field: SerializeField] public bool IsInstance { get; private set; }

    public static event Action GenerationEnd;

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
        DisableCollider();
    }

    [ServerRpc(RequireOwnership = false)]
    public void Generate()
    {
        GenerateMulticast();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsInstance)
            return;
        if (other.TryGetComponent<Generator>(out Generator generator))
        {
            Debug.LogError(gameObject.name);
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
        PlayerCharacter character = PlayerCharacter.Instance;

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