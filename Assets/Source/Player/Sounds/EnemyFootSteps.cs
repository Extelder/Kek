using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EnemyFootSteps : NetworkBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip[] _audioClip;
    private AudioClip _oldclip;
    private int _lastIndex;
    private int _lastTwoIndex;
    [ServerRpc(RequireOwnership = false)]
    public void RandomizeFootSteps()
    {
        RandomizeObserver();
    }

    public int GetRandomIndex()
    {
        int index = Random.Range(0, _audioClip.Length - 1);
        if (index == _lastIndex)
        {
            return GetRandomIndex();
        }
        if (index == _lastTwoIndex)
        {
            return GetRandomIndex();
        }

        _lastTwoIndex = _lastIndex;
        return index;
    }

    [ObserversRpc]
    private void RandomizeObserver()
    {
        _lastIndex = GetRandomIndex();
        _audio.clip = _audioClip[_lastIndex];
        _audio.Play();
    }
}