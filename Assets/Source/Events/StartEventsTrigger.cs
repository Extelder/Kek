using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEventsTrigger : MonoBehaviour
{
    public event Action Triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerHitBox>(out PlayerHitBox PlayerHitBox))
        {
            Triggered?.Invoke();
        }
    }
}