using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ItemTakeUp : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _takeUp;
    public bool TakeUpped { get; private set; }

    private void OnEnable()
    {
        TakeUpped = false;
    }

    public void TakeUp()
    {
        _animator.SetTrigger(_takeUp);
    }

    public void TakeUpAnimationEnd()
    {
        TakeUpped = true;
    }
}