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
    
    private void OnEnable()
    {
        _drill.StartedDrilling += OnStartedDrilling;
        _drill.StoppedDrilling += OnStoppedDrilling;
        _animator.Hitted += OnHitted;
        _animator.NotHitted += OnNotHitted;
    }

    private void OnStoppedDrilling()
    {
        _secondSoundPlayPause.Pause(true);
        _soundPlayPause.Pause(true);
    }

    private void OnStartedDrilling(bool value)
    {
        _secondSoundPlayPause.Pause(value);
        _soundPlayPause.Pause(true);
    }

    private void OnHitted()
    {
        _soundPlayPause.Pause(false);
    }
    
    private void OnNotHitted()
    {
        StopSound();
    }

    private void StopSound()
    {
        _soundPlayPause.Pause(true);
    }

    private void OnDisable()
    {
        _drill.StartedDrilling -= OnStartedDrilling;
        _drill.StoppedDrilling -= OnStoppedDrilling;
        _animator.Hitted -= OnHitted;
        _animator.NotHitted -= OnNotHitted;
        StopSound();
    }
}
