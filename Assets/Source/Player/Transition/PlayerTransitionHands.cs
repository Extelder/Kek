using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransitionHands : MonoBehaviour
{
    [SerializeField] private Transform _origin;
    [SerializeField] private float _speed;

    [SerializeField] private Animator _animator;
    [SerializeField] private string _triggerName;
    
    private void OnEnable()
    {
        VendingMachineItem.Interacted += OnInteracted;
    }

    private void OnInteracted(Transform target)
    {
        _origin.position = Vector3.MoveTowards(_origin.position, target.position, _speed);
        _animator.SetTrigger(_triggerName);
    }

    private void OnDisable()
    {
        VendingMachineItem.Interacted -= OnInteracted;
    }
}
