using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class LocationChanger : NetworkBehaviour
{
    [SerializeField] private float _waitTIme;

    [SerializeField] private PlayersInChecker _playerInChecker;

    [SerializeField] private Generator _generator;
    [SerializeField] private NetWorkAnimatorSynchronize _netWorkAnimatorSynchronize;
    [SerializeField] private NetWorkAnimatorSynchronize _controlPanelAnimator;

    private void OnEnable()
    {
        _playerInChecker.AllPlayerInAction += OnAllPlayerInAction;
    }

    private void OnAllPlayerInAction()
    {
        _netWorkAnimatorSynchronize.SetAnimatorBool("IsOpen", false);
        _generator.ReGenerate();
        _controlPanelAnimator.SetAnimatorBool("IsActivate", false);
        _netWorkAnimatorSynchronize.SetBlock(true);
        Invoke(nameof(EnableDoor), _waitTIme);
    }

    private void EnableDoor()
    {
        _netWorkAnimatorSynchronize.SetBlock(false);
    }


    private void OnDisable()
    {
        _playerInChecker.AllPlayerInAction -= OnAllPlayerInAction;
    }
}