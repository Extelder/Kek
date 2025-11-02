using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class MixSoundAndPlay : NetworkBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip[] _audioClips;


    [ServerRpc(RequireOwnership = false)]
    public void MixOnServer()
    {
        MixAndPlayObserver();
    }
    
    [ObserversRpc]
    private void MixAndPlayObserver()
    {
        _audio.clip = _audioClips[Random.Range(0, _audioClips.Length - 1)];
        _audio.Play();
    }
}
