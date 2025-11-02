using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEyesLookAt : MonoBehaviour
{
    private void Update()
    {
        PlayerCharacter[] characters = GameObject.FindObjectsOfType<PlayerCharacter>();

        foreach (var character in characters)
        {
            if(character == null)
                continue;
            
        }
    }
}
