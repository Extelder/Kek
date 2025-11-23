using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundEventBazooka : NetworkBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip[] _audioClip;
    private AudioClip _oldclip;
    private int _lastIndex;
    private int _lastTwoIndex;

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
    
    public IEnumerator FootStep()
    {
        while (true)
        {
            float random = Random.Range(0.5f, 0.6f);
            yield return new WaitForSeconds(random);
            _lastIndex = GetRandomIndex();
            _audio.clip = _audioClip[_lastIndex];
            _audio.Play();
            float randomChance = Random.Range(1f, 100f);
        }
    }
}
