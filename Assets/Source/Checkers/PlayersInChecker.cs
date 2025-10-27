using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using UnityEngine;

public class PlayersInChecker : MonoBehaviour
{
    [SerializeField] private int _playersIn;

    public event Action AllPlayerInAction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerCharacter))
        {
            _playersIn++;
            Debug.Log(AllPlayersIn());
            if (AllPlayersIn())
            {
                AllPlayerInAction?.Invoke();
            }
        }
    }

    public bool AllPlayersIn() => _playersIn == NetworkManager.Instances[0].ClientManager.Clients.Count;

    private void OnDisable()
    {
        _playersIn = 0;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerCharacter))
        {
            _playersIn--;
        }
    }
}