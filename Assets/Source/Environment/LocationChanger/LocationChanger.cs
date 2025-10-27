using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationChanger : MonoBehaviour
{
    [SerializeField] private PlayersInChecker _playerInChecker;

    [SerializeField] private Generator _generator;

    private void OnEnable()
    {
        _playerInChecker.AllPlayerInAction += OnAllPlayerInAction;
    }

    private void OnAllPlayerInAction()
    {
        _generator.ReGenerate();
    }

    private void OnDisable()
    {
        _playerInChecker.AllPlayerInAction -= OnAllPlayerInAction;
    }
}