using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class BazookaBullet : NetworkBehaviour
{
    [SerializeField] private float _speed;
    public PlayerCharacter target;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioSource _audioFly;
    [SerializeField] private GameObject _bulletOff;
    [SerializeField] private Collider _bulletCollider;
    [SerializeField] private ParticleSystem _gasFx;

    private void Start()
    {
        _audioFly.Play();
        StartCoroutine(AimStop());
    }

    private void Update()
    {
        if (target == null)
        {
            transform.position += transform.forward * _speed * Time.deltaTime;
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 100f * 100f * Time.deltaTime);
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    [ObserversRpc]
    private void OnDestroyObserver()
    {
        _audio.Play();
        _bulletOff.SetActive(false);
        _bulletCollider.enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnDestroyServer()
    {
        OnDestroyObserver();
        PlayerCharacter.Instance.PlayerHitBox.TakeDamage(10);
        StartCoroutine(DestroyDelay());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            OnDestroyServer();
        }
    }

    private IEnumerator DestroyDelay()
    {
        _audioFly.Stop();
        _gasFx.Play();
        yield return new WaitForSeconds(2.1f);
        PlayerCharacter.Instance.Despawn(gameObject);
    }

    private IEnumerator AimStop()
    {
        yield return new WaitForSeconds(0.5f);
        target = null;
    }
}