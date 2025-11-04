using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerRotation : NetworkBehaviour
{
    [SerializeField] private PlayerCharacter _character;
    [SerializeField] private Transform _camera;

    public bool CopyAllRotation;

    private void FixedUpdate()
    {
        if (base.IsOwner == false)
            return;
        if (CopyAllRotation)
        {
            transform.localEulerAngles = _camera.localEulerAngles;
            return;
        }

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, _camera.localEulerAngles.y,
            transform.localEulerAngles.z);
    }
}