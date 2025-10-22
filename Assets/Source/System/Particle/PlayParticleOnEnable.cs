using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleOnEnable : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particle;

    private void OnEnable()
    {
        _particle.Play();
    }
}
