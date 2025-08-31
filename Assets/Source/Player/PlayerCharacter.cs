using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [field: SerializeField] public Rigidbody Rigidbody;
    [field: SerializeField] public PlayerBinds Binds;
    [field: SerializeField] public Transform PlayerTransform;
    
    public static PlayerCharacter Instance { get; private set; }

    
    private void Awake()
    {
        if (!Instance)
        {
            Binds = new PlayerBinds();

            Binds.Enable();

            Instance = this;
            return;
        }

        Destroy(this);
    }

    private void OnDisable()
    {
        Binds.Dispose();
        Binds.Disable();
    }
}