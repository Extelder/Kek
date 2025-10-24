using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using NaughtyAttributes;
using UnityEngine;

public class Generator : NetworkBehaviour
{
    [SerializeField] private GameObject _block;

    [SerializeField] private GameObject[] _spawnableParts;

    [SerializeField] private Transform[] _spawnNextPoints;

    [field: ShowIf(nameof(IsInstance))] public int MaxGenerateParts { get; private set; }
    [field: ShowIf(nameof(IsInstance))] public int SpawnedGenerateParts { get; private set; }

    public static Generator Instance { get; private set; }

    [field: SerializeField] public bool IsInstance { get; private set; }

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
        }
    }

    [ServerRpc]
    public void Generate()
    {
        GenerateMulticast();
    }

    [ObserversRpc]
    public void GenerateMulticast()
    {
        PlayerCharacter character = PlayerCharacter.Instance;

        for (int i = 0; i < _spawnNextPoints.Length; i++)
        {
            if (Instance.SpawnedGenerateParts >= Instance.MaxGenerateParts)
            {
                character.ServerSpawnObject(_block, _spawnNextPoints[i].position,
                    Quaternion.LookRotation(_spawnNextPoints[i].forward));
                return;
            }

            Instance.SpawnedGenerateParts++;
            GameObject part = (_spawnableParts[Random.Range(0, _spawnableParts.Length)]);

            character.ServerSpawnObject(part, _spawnNextPoints[i].position,
                Quaternion.LookRotation(_spawnNextPoints[i].forward));
        }
    }
}