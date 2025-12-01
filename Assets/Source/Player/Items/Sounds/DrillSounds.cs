using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillSounds : MonoBehaviour
{
    [SerializeField] private Drill _drill;
    [SerializeField] private SoundPlayPause _soundPlayPause;
    
    private void OnEnable()
    {
        _drill.StartedDrilling += OnStartedDrilling;
        _drill.StoppedDrilling += OnStoppedDrilling;
        _drill.NotEquiped += OnNotEquiped;
    }

    private void OnNotEquiped()
    {
        _soundPlayPause.Pause(true);
    }

    private void OnStoppedDrilling()
    {
        _soundPlayPause.Pause(true);
    }

    private void OnStartedDrilling(bool value)
    {
        _soundPlayPause.Pause(value);
    }

    private void OnDisable()
    {
        _drill.StartedDrilling -= OnStartedDrilling;
        _drill.StoppedDrilling -= OnStoppedDrilling;
        _drill.NotEquiped -= OnNotEquiped;
    }
}
