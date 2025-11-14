using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloorRandomEvent : RandomEvent
{
    [SerializeField] private RaycastSettings _raycastSettings;
    [SerializeField] private GameObject _spawnObject;
    
    public override void StartEvent()
    {
        Transform spawnPoint =
            Generator.Instance.SpawnedEnemySpawnPoint[Random.Range(0, Generator.Instance.SpawnedEnemySpawnPoint.Count)];
        bool hitted = Physics.Raycast(spawnPoint.position, Vector3.down, out RaycastHit hit,
            _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
        if (hitted)
        {
            if (hit.collider.TryGetComponent<Ground>(out Ground ground))
            {
                PlayerCharacter.Instance.ServerSpawnObject(_spawnObject, spawnPoint.position, Quaternion.FromToRotation(transform.up ,hit.normal));
            }
        }
    }
}
