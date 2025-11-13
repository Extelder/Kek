using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaBullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    public PlayerCharacter target;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioSource _audioFly;
    [SerializeField] private GameObject _bulletOff;
    [SerializeField] private Collider _bulletCollider;

    private void Start()
    {
        _audioFly.Play();
    }

    void Update()
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

    void OnCollisionEnter(Collision collision)
    {
        var player = collision.collider.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            _audio.Play();
            _bulletOff.SetActive(false);
            _bulletCollider.enabled = false;
            StartCoroutine(DestroyDelay());
        }
        
    }

    private IEnumerator DestroyDelay()
    {
        _audioFly.Stop();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}