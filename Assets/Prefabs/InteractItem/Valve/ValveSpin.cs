using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class ValveSpin : NetworkBehaviour
{
    [SerializeField] private GameObject _valveObject;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private float _speed;
    [SerializeField] private float _maxAngle;
    private bool IsSpin;
    private float _currentAngle;
    private bool canSpin = true;
    private void Start()
    {
        _audio.Play();
        _audio.Pause();
    }
    [ServerRpc(RequireOwnership = false)]
    public void Press()
    {
        if (canSpin) ObserverPress(true);
    }

    private void ObserverPress(bool press)
    {
        if (press)
        {
            _audio.UnPause();
            IsSpin = true;
            return;
        }
        _audio.Pause();
        IsSpin = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UnPress()
    {
        ObserverPress(false);
    }
    [ObserversRpc]
    private void SpinComplete()
    {
        Debug.Log("OKDOSKDOASDASDASDAD");
    }

    private void Update()
    {
        if (IsSpin)
        {
            if (_currentAngle < _maxAngle)
            {
                float delta = _speed * Time.deltaTime;
                if (_currentAngle + delta > _maxAngle)
                    delta = _maxAngle - _currentAngle;
                _currentAngle += delta;
                transform.localEulerAngles += new Vector3(0f, 0f, delta);
            }
            else
            {
                canSpin = false;
                IsSpin = false;
                _audio.Pause();
                if (IsServer)
                {
                    SpinComplete();
                }
            }
        }
    }
}
