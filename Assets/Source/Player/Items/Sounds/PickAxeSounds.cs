using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAxeSounds : MonoBehaviour
{
    [SerializeField] private MineableItemAnimator _animator;
    [SerializeField] private MixSoundAndPlay _mixSound;
    
    private void OnEnable()
    {
        _animator.Hitted += OnHitted;
    }
    
    private void OnHitted()
    {
        _mixSound.MixOnServer();
    }

    private void OnDisable()
    {
        _animator.Hitted -= OnHitted;
    }
}
