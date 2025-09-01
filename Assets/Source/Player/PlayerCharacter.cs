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

    public static PlayerCharacter Instance { get; private set; }

    public event Action ClientStarted;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            Binds = new PlayerBinds();

            Binds.Enable();

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