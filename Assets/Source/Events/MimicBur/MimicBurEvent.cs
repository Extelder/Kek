using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class MimicBurEvent : RandomEvent
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _enemySpawnPoints;

    [SerializeField] private Vector3 _spawnOffset;

    [SerializeField] private Transform _bur;
    [SerializeField] private Vector2 _spawnTimeRandomRateRange;
    [SerializeField] private float _activeTime = 40f;
    [SerializeField] private float _burSpeed;

    private CompositeDisposable _disposable = new CompositeDisposable();

    private Tween _moveUp;

    public override void StartEvent()
    {
        StartCoroutine(Spawning());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _disposable?.Clear();
        _moveUp?.Kill();
    }

    private IEnumerator Spawning()
    {
        while (true)
        {
            float randomRate = Random.Range(_spawnTimeRandomRateRange.x, _spawnTimeRandomRateRange.y);
            yield return new WaitForSeconds(randomRate);
            _disposable?.Clear();

            PlayerCharacter randomCharacter =
                PlayerCharacter.Instance.Characters[Random.Range(0, PlayerCharacter.Instance.Characters.Count)];
            Vector3 targetPoint = randomCharacter.PlayerTransform.position;
            _bur.position = targetPoint - _spawnOffset;

            _moveUp = _bur.DOMove(targetPoint, _burSpeed).OnComplete(() =>
            {
                for (int i = 0; i < _enemySpawnPoints.Length; i++)
                {
                    PlayerCharacter.Instance.ServerSpawnObject(_enemyPrefab, _enemySpawnPoints[i].position,
                        Quaternion.identity);
                }
            });

            yield return new WaitForSeconds(_activeTime);
            _moveUp?.Kill();
            _disposable?.Clear();
            targetPoint -= _spawnOffset * 2f;
            Observable.EveryUpdate().Subscribe(_ =>
                {
                    _bur.Translate(-transform.up * _burSpeed * Time.deltaTime, Space.World);
                    if (_bur.position.y >= targetPoint.y)
                    {
                        _disposable?.Clear();
                    }
                })
                .AddTo(_disposable);
        }
    }
}