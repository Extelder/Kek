using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerHitBox : MonoBehaviour
{
    [SerializeField] private float _notActiveDelayAfterSpawn;

    private bool _active;
    private PlayerHealth _health;

    private void Start()
    {
        _health = PlayerCharacter.Instance.PlayerHealth;
        StopAllCoroutines();
        StartCoroutine(WaitForDelay());
    }

    private IEnumerator WaitForDelay()
    {
        yield return new WaitForSeconds(_notActiveDelayAfterSpawn);
        _active = true;
    }

    public virtual void TakeDamage(float damage)
    {
        if (!_active)
            return;
        _health.TakeDamage(damage);
    }
}