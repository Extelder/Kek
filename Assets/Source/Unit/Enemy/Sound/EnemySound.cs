using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySound : MonoBehaviour
{
    [SerializeField] private AudioSource _soundsStep;
    [SerializeField] private AudioSource _sounds2;
    private bool _move;
    private bool _attack;

    private void Start()
    {
        _soundsStep.Play();
        _soundsStep.Pause();
        _sounds2.Play();
        _sounds2.Pause();
    }

    private void UpdateVariable()
    {
        if (_move)
        {
            _soundsStep.UnPause();
        }
        else
        {
            {
                _soundsStep.Pause();
            }
        }

        if (_attack)
        {
            _sounds2.Play();
        }
    }

    public void Move()
    {
        _move = true;
        _attack = false;
        UpdateVariable();
    }

    public void Run()
    {
        _move = true;
        _attack = false;
        UpdateVariable();
    }

    public void Attack()
    {
        _move = false;
        _attack = true;
        UpdateVariable();
    }

    public void Idle()
    {
        _move = false;
        _attack = false;
        UpdateVariable();
    }
}