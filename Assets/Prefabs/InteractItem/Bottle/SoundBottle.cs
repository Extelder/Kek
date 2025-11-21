using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class SoundBottle : NetworkBehaviour
{
    [SerializeField] private InteractItem _boxInteract;
    private BoxInteract interact => (BoxInteract) _boxInteract.Item;
    [SerializeField] private AudioSource _audio;
    
    private void OnEnable()
    {
        interact.InteractExplosion += Interact;
    }

    private void OnDisable()
    {
        interact.InteractExplosion -= Interact;
    }

    private void Interact()
    {
        InteractServer();
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractServer()
    {
        InteractObserver();
    }
    [ObserversRpc]
    private void InteractObserver()
    {
        _audio.Play();
    }
    
    
}
