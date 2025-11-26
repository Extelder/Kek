using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnableTrap : PlayerTrigger
{
    [SerializeField] private GameObject _objectOriginal;
    [SerializeField] private GameObject _objectTrap;
    [SerializeField] private float _delay;

    public override void OnTriggered(PlayerHealth playerHealth)
    {
        if(Random.value <= 0.9) StartCoroutine(Triggering());
    }

    private IEnumerator Triggering()
    {
         yield return new WaitForSeconds(_delay);
         _objectOriginal.SetActive(false);
         _objectTrap.SetActive(true);
    }
}
