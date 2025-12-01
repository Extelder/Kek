using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EnemyFootSteps : MonoBehaviour
{
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip[] _audioClipRock;
    [SerializeField] private AudioClip[] _audioClipIron;
    [SerializeField] private GroundChecker _ground;
    [SerializeField] private float _VolumeIron;
    [SerializeField] private float _VolumeRock;
    private AudioClip _oldclip;
    private int _lastIndex;

    public void RandomizeFootSteps()
    {
        RandomizeObserver(_ground.Foot);
    }

    public int GetRandomIndexRock()
    {
        int index = Random.Range(0, _audioClipRock.Length - 1);
        if (index == _lastIndex)
        {
            return GetRandomIndexRock();
        }

        return index;
    }

    public int GetRandomIndexIron()
    {
        int index = Random.Range(0, _audioClipRock.Length - 1);
        if (index == _lastIndex)
        {
            return GetRandomIndexIron();
        }

        return index;
    }

    private void RandomizeObserver(FootStep foot)
    {
        if (foot == FootStep.iron)
        {
            _lastIndex = GetRandomIndexIron();
            _audio.clip = _audioClipIron[_lastIndex];
            _audio.volume = _VolumeIron;
            _audio.Play();
            return;
        }

        if (foot == FootStep.rock)
        {
            _lastIndex = GetRandomIndexRock();
            _audio.clip = _audioClipRock[_lastIndex];
            _audio.volume = _VolumeRock;
            _audio.Play();
        }
    }
}