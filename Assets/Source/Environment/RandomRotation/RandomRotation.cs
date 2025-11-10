using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomRotation : MonoBehaviour
{
    [SerializeField] private float _speed;

    private void Start()
    {
        if (Random.value > 0.5f)
        {
            _speed *= -1f;
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * _speed, Space.Self);
    }
}