using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EnemyFootSteps : NetworkBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip[] _audioClip;
    
    [ServerRpc(RequireOwnership = false)]
    public void RandomizeFootSteps()
    {
        RandomizeObserver();
    }
    [ObserversRpc]
    private void RandomizeObserver()
    {
        _audio.clip = _audioClip[Random.Range(0, _audioClip.Length - 1)];
        _audio.Play();
    }
}