using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Radio : NetworkBehaviour, IInteractable
{
    [SerializeField] private AudioSource _audioSource;

    public void Interact()
    {
        SetSoundPause(!_audioSource.isPlaying);
    }

    public void InteractCancelled()
    {
        
    }

    [ServerRpc]
    public void SetSoundPause(bool pause)
    {
        SetSoundPauseObserver(pause);
    }

    [ObserversRpc]
    public void SetSoundPauseObserver(bool pause)
    {
        if (pause)
            _audioSource.Pause();
        else
            _audioSource.UnPause();
    }
}