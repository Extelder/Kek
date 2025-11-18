using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitRandomPlayerDetector : NetworkBehaviour
{
    [SerializeField] private Vector2 _randomDelay = new Vector2(5f, 25f);

    [SerializeField] private EnemyStateMachine _enemyStateMachine;

    public override void OnStartClient()
    {
        if (!base.IsServer)
            return;
        StartChecking();
    }

    public void StartChecking()
    {
        StartCoroutine(Randoming());
    }

    private IEnumerator Randoming()
    {
        while (true)
        {
            yield return new WaitUntil(() => PlayerCharacter.Instance != null);
            PlayerCharacter randomCharacter =
                PlayerCharacter.Instance.Characters[Random.Range(0, PlayerCharacter.Instance.Characters.Count)];
            _enemyStateMachine?.Chase(randomCharacter.PlayerMovement.transform);
            yield return new WaitForSeconds(Random.Range(_randomDelay.x, _randomDelay.y));
        }
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }
}