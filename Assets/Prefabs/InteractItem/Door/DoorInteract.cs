using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    [SerializeField] private ValveSpin _valve;
    [SerializeField] private float _speed;
    [SerializeField] private float _maxAngle;
    [SerializeField] private GameObject _object;
    private bool IsSpin;
    private float _currentAngle;

    private void Start()
    {
        if (_maxAngle < 0)
        {
            _speed = _speed * -1f;
        }
    }

    private void OnEnable()
    {
        _valve.SpinCompleteEvent += ValveSpinComplete;
    }

    private void OnDisable()
    {
        _valve.SpinCompleteEvent -= ValveSpinComplete;
    }

    private void ValveSpinComplete()
    {
        IsSpin = true;
    }

    private void Update()
    {
        if (!IsSpin)
            return;

        float delta = _speed * Time.deltaTime;

        if (_maxAngle >= 0)
        {
            if (_currentAngle < _maxAngle)
            {
                if (_currentAngle + delta > _maxAngle)
                    delta = _maxAngle - _currentAngle;

                _currentAngle += delta;
                _object.transform.localEulerAngles += new Vector3(0f, 0f, delta);
            }
            else
            {
                IsSpin = false;
            }
        }
        else
        {
            if (_currentAngle > _maxAngle)
            {
                if (_currentAngle + delta < _maxAngle)
                    delta = _maxAngle - _currentAngle;

                _currentAngle += delta;
                _object.transform.localEulerAngles += new Vector3(0f, 0f, delta);
            }
            else
            {
                IsSpin = false;
            }
        }
    }
}