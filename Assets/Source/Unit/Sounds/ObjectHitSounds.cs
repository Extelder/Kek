using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ObjectHitSounds : MonoBehaviour
{
    [SerializeField] private HitBox _hitBox;
    [SerializeField] private MixSoundAndPlay _mixSoundPickAxe;
    
    [SerializeField] private bool _mixSoundDrillUsable = true;
    [ShowIf(nameof(_mixSoundDrillUsable)), SerializeField] private MixSoundAndPlay _mixSoundDrill;

    [SerializeField] private bool _soundPlayPauseUsable;
    [ShowIf(nameof(_soundPlayPauseUsable)), SerializeField] private SoundPlayPause _soundPlayPause;

    private void OnEnable()
    {
        _hitBox.DrillHitted += OnDrillHitted;
        _hitBox.PickAxeHitted += OnPickAxeHitted;
    }

    private void OnDrillHitted()
    {
        if (_soundPlayPauseUsable)
        {
            StopAllCoroutines();
            StartCoroutine(PlaySound());
        }
        _mixSoundDrill?.MixOnServer();
    }

    private IEnumerator PlaySound()
    {
        _soundPlayPause?.Pause(false);
        yield return new WaitForSeconds(0.1f);
        _soundPlayPause?.Pause(true);
    }

    private void OnPickAxeHitted()
    {
        _mixSoundPickAxe?.MixOnServer();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _hitBox.DrillHitted -= OnDrillHitted;
        _hitBox.PickAxeHitted -= OnPickAxeHitted;
    }
}
