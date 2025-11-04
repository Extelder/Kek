using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDestroyed : MonoBehaviour
{
    [SerializeField] private Ore _ore;
    [SerializeField] private Rigidbody[] _rigidbodies;

    private void OnEnable()
    {
        _ore.Destroyed += OnDestroyed;
    }

    private void OnDestroyed()
    {
        for (int i = 0; i < _rigidbodies.Length; i++)
        {
            _rigidbodies[i].useGravity = true;
        }
    }

    private void OnDisable()
    {
        _ore.Destroyed -= OnDestroyed;
    }
}
