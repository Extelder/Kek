using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class SoundPlayPause : NetworkBehaviour
{
    [SerializeField] private AudioSource _audio;
    public override void OnStartClient()
    {
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
        if (pause) _audio.Pause();
            else _audio.UnPause();
    }
}