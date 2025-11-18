using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lamp : EquipItem
{
    [SerializeField] private GameObject _lampLight;
    [SerializeField] private GameObject _lampLightRPC;
    [SerializeField] private AudioSource _Sound;
    [SerializeField] private AudioClip _On;
    [SerializeField] private AudioClip _Off;

    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        PlayerCharacter.Instance.SetObjectEnableServer(_lampLightRPC, !_lampLightRPC.activeSelf);
        _lampLight.SetActive(!_lampLight.activeSelf);
        _Sound.clip = _On;
        _Sound.Play();
    }

    public override void OnEquipStateChanged()
    {
        base.OnEquipStateChanged();
        Debug.Log("PickUp");
        PlayerAnimator.PickUpAnim();
    }

    private void Update()
    {
        if (_equiped)
        {
            PlayerAnimator.PickUpAnim();
        }
    }

    public override void OnInputCanceled(InputAction.CallbackContext obj)
    {
        _Sound.clip = _Off;
        _Sound.Play();
    }
}