using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class OreGenerator : NetworkBehaviour
{
    [SerializeField] private GameObject[] _ores;

    public override void OnStartClient()
    {
        if (!IsServer)
            return;

        Generator.GenerationEnd += OnGenerationEnd;
    }

    private void OnDisable()
    {
        if (!IsServer)
            return;

        Generator.GenerationEnd -= OnGenerationEnd;
    }

    private void OnGenerationEnd()
    {
        for (int i = 0; i < Generator.Instance.OreSpawnPlaces.Count; i++)
        {
            if (Generator.Instance.OreSpawnPlaces[i] == null)
                continue;

            PlayerCharacter.Instance.ServerSpawnObject(_ores[Random.Range(0, _ores.Length)],
                Generator.Instance.OreSpawnPlaces[i].transform.position,
                Quaternion.LookRotation(Generator.Instance.OreSpawnPlaces[i].transform.position));
        }
    }
}