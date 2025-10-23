using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [field: SerializeField] public Rigidbody Rigidbody;
    [field: SerializeField] public PlayerBinds Binds;
    [field: SerializeField] public Transform PlayerTransform;
    [field: SerializeField] public GameObject[] _thirdPerson;

    public static PlayerCharacter Instance { get; private set; }

    public event Action ClientStarted;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            Binds = InputManager.inputActions;

            Binds.Enable();

            for (int i = 0; i < _thirdPerson.Length; i++)
            {
                _thirdPerson[i].SetActive(false);
            }

            Instance = this;
        }

        ClientStarted?.Invoke();
    }

    private void OnDisable()
    {
        if (!base.IsOwner)
            return;
        Binds?.Disable();
    }
}