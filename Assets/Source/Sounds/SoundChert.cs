using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class SoundChert : NetworkBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private CollectTrigger _qouta;

    private void OnEnable()
    {
        _qouta.ItemEatable += ItemEat;
    }

    private void OnDisable()
    {
        _qouta.ItemEatable -= ItemEat;
    }


    private void ItemEat()
    {
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
        _audio.Play();
    }
}