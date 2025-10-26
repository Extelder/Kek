using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TNT : EquipItem
{
    [SerializeField] private GameObject _tntThrowablePrefab;
    public override void OnAttackInputReceived(InputAction.CallbackContext obj)
    {
        PlayerAnimator.ThrowAnim();
        PlayerCharacter.Instance.ServerSpawnObject(_tntThrowablePrefab, PlayerCharacter.Instance.DropPoint.position, 
            PlayerCharacter.Instance.CameraTransform.rotation);
    }
}
