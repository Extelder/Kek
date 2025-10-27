using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public abstract class PlayerTrigger : NetworkBehaviour
{

    [SerializeField] private bool _destroyGameObjectAfterTriggered;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            OnTriggered(playerHealth);
            if (_destroyGameObjectAfterTriggered)
            {
                Despawn(gameObject);
            }
        }
    }

    public abstract void OnTriggered(PlayerHealth playerHealth);

}
