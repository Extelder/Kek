using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpad : MonoBehaviour
{
    [SerializeField] private float _force;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerCharacter))
        {
            PlayerCharacter.Rigidbody.AddForce(Vector3.up * _force, ForceMode.Impulse);
        }
    }
}