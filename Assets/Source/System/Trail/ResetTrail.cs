using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTrail : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;
    private float _defaultTime;
    
    private void OnEnable()
    {
        _defaultTime = _trailRenderer.time;
        _trailRenderer.time = -1;
        Invoke(nameof(Reset), 0.02f);
    }

    private void Reset()
    {
        _trailRenderer.time = _defaultTime;
    }
}
