using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillSounds : MonoBehaviour
{
    [SerializeField] private MineableItemAnimator _animator;
    [SerializeField] private Drill _drill;
    [SerializeField] private SoundPlayPause _soundPlayPause;
    [SerializeField] private SoundPlayPause _secondSoundPlayPause;
    private bool _musicPlay;
    
    private void OnEnable()
    {
        _drill.StartedDrilling += OnStartedDrilling;
        _animator.Hitted += OnHitted;
        _animator.NotHitted += OnNotHitted;
    }

    private void OnStartedDrilling(bool value)
    {
        _secondSoundPlayPause.Pause(value);
        _soundPlayPause.Pause(true);
    }

    private void OnHitted()
    {
        if (!_musicPlay)
        {
            _soundPlayPause.Pause(false);
            _musicPlay = true;
        }
    }
    
    private void OnNotHitted()
    {
        StopSound();
    }

    private void StopSound()
    {
        if (_musicPlay)
        {
            _soundPlayPause.Pause(true);
            _musicPlay = false;
        }
    }

    private void OnDisable()
    {
        _drill.StartedDrilling -= OnStartedDrilling;
        _animator.Hitted -= OnHitted;
        _animator.NotHitted -= OnNotHitted;
        StopSound();
    }
}
