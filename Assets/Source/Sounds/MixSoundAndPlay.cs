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
    [SerializeField] private bool _DestroyAfterPlay;
    private bool _playing;

    public override void OnStartClient()
    {
        if (_DestroyAfterPlay)
            MixOnServer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void MixOnServer()
    {
        MixAndPlayObserver();
    }

    [ObserversRpc]
    private void MixAndPlayObserver()
    {
        if (_audio == null)
            return;
        _audio.clip = _audioClips[Random.Range(0, _audioClips.Length)];
        _audio.Play();
        if(_DestroyAfterPlay)
            _playing = true;
    }

    private void Update()
    {
        if (_playing)
        {
            if (!_audio.isPlaying)
            {
                Despawn(gameObject);
            }
        }
    }
}