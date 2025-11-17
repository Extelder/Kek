using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAxeSounds : MonoBehaviour
{
    [SerializeField] private MineableItemAnimator _animator;
    [SerializeField] private MixSoundAndPlay _mixSound;
    [SerializeField] private MixSoundAndPlay _mixSoundSecond;
    
    private void OnEnable()
    {
        _animator.Hitted += OnHitted;
        _animator.NotHitted += OnNotHitted;
    }
    
    private void OnHitted()
    {
        _mixSound.MixOnServer();
        _mixSoundSecond.MixOnServer();
    }
    
    private void OnNotHitted()
    {
        _mixSoundSecond.MixOnServer();
    }

    private void OnDisable()
    {
        _animator.Hitted -= OnHitted;
        _animator.NotHitted -= OnNotHitted;
    }
}
