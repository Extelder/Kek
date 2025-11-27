using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class OreGenerator : NetworkBehaviour
{
    [SerializeField] private GameObject[] _ores;

    private bool _generating;

    public override void OnStartClient()
    {
        if (!IsServer)
            return;

        Generator.GenerationEnd += OnGenerationEnd;
        Generator.Instance.Regenerate += OnRegenerate;
    }

    private void OnRegenerate()
    {
        _generating = false;
    }

    private void OnDisable()
    {
        if (!IsServer)
            return;

        Generator.GenerationEnd -= OnGenerationEnd;
        Generator.Instance.Regenerate -= OnRegenerate;
    }

    private void OnGenerationEnd()
    {
        Debug.LogError("Invoked");


        if (_generating)
            return;
        _generating = true;

        for (int i = 0; i < Generator.Instance.OreSpawnPlaces.Count; i++)
        {
            if (Generator.Instance.OreSpawnPlaces[i] == null)
                continue;
            Debug.LogError("Spawned");
            Vector3 basePos = Generator.Instance.OreSpawnPlaces[i].transform.position;
            Vector3 spawnPos = basePos + new Vector3(0, 0.1f, 0);

            RaycastHit hit;
            Vector3 finalNormal = Vector3.forward;

            if (Physics.Raycast(basePos + Vector3.forward * 0.1f, -Vector3.forward, out hit, 1f))
            {
                finalNormal = hit.normal;
            }

            Quaternion rot = Quaternion.LookRotation(finalNormal, Vector3.up);

            PlayerCharacter.Instance.ServerSpawnObject(
                _ores[Random.Range(0, _ores.Length)],
                spawnPos,
                rot
            );
        }
    }
}