using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class DestroyWithoutOwner : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _character;

    private void OnEnable()
    {
        _character.ClientStarted += OnClienStarted;
    }

    private void OnClienStarted()
    {
        if (!_character.IsOwner)
            Destroy(gameObject);
        else
        {
            transform.SetParent(null);
            transform.parent = null;
        }
    }

    private void OnDisable()
    {
        _character.ClientStarted -= OnClienStarted;
    }
}