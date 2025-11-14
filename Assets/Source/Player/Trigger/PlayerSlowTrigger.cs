using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using UnityEngine;

public class PlayerSlowTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacter.Instance.PlayerMovement.Decceleration = 0;
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerCharacter.Instance.PlayerMovement.Decceleration = 128;
    }
}
