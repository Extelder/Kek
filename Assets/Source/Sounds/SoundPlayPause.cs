using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundPlayPause : NetworkBehaviour
{
    [SerializeField] private AudioSource _audio;

    public override void OnStartClient()
    {
        if (_audio == null)
            return;
        _audio.Play();
        _audio.Pause();
    }

    [ServerRpc(RequireOwnership = false)]
    public void Pause(bool pause)
    {
        PauseMulticast(pause);
    }

    [ObserversRpc]
    private void PauseMulticast(bool pause)
    {
        if (_audio == null)
            return;
        if (pause)
        {
            _audio.Pause();
        }
        else
        {
            _audio.UnPause();
        }
    }
}