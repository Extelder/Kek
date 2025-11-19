using System;
using System.Collections;
using FishNet.Object;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitRandomPlayerDetector : NetworkBehaviour
{
    [SerializeField] private Vector2 _randomDelay = new Vector2(5f, 25f);

    [SerializeField] private EnemyStateMachine _enemyStateMachine;
    [SerializeField] private float _playerUpdateRate;

    private CompositeDisposable _disposable = new CompositeDisposable();

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

            Observable.Interval(TimeSpan.FromSeconds(_playerUpdateRate)).Subscribe(_ =>
            {
                _enemyStateMachine?.Chase(randomCharacter.PlayerMovement.transform);
            }).AddTo(_disposable);
            yield return new WaitForSeconds(Random.Range(_randomDelay.x, _randomDelay.y));
            _disposable?.Clear();
        }
    }

    public void OnDisable()
    {
        _disposable?.Clear();
        StopAllCoroutines();
    }
}