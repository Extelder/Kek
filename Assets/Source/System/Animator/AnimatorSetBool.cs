using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetBool : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _animBoolValue;
    [SerializeField] private string _animBoolName;

    public void SetBool()
    {
        _animator.SetBool(_animBoolName, _animBoolValue);
    }
}
