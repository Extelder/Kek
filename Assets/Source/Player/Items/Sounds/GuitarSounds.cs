using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuitarSounds : MonoBehaviour
{
    [SerializeField] private Guitar _guitar;
    [SerializeField] private SoundPlayPause _music;

    private void OnEnable()
    {
        _guitar.StartedPlayingMusic += OnMusicStartedPlaying;
        _guitar.StoppedPlayingMusic += OnMusicStoppedPlaying;
        _guitar.NotEquiped += OnNotEquiped;
    }

    private void OnNotEquiped()
    {
        _music.Pause(true);
    }

    private void OnMusicStoppedPlaying()
    {
        _music.Pause(true);
    }

    private void OnMusicStartedPlaying(bool value)
    {
        _music.Pause(value);
    }

    private void OnDisable()
    {
        _guitar.StartedPlayingMusic -= OnMusicStartedPlaying;
        _guitar.StoppedPlayingMusic -= OnMusicStoppedPlaying;
        _guitar.NotEquiped -= OnNotEquiped;
    }
}
