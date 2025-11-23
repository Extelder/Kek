using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KariteyDefender3000;

public class RequireComponentChecker : MonoBehaviour
{
    private void Start()
    {
        SafeUtils.Require(PlayerCharacter.Instance, "Игрок", true);
    }
}
