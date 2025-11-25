using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lamp : EquipItem
{
    [SerializeField] private Color _startColor;
    [SerializeField] private float _emission;
    [SerializeField] private Material _lampMaterial;
    [SerializeField] private GameObject _lampLight;
    [SerializeField] private GameObject _lampLightRPC;
    [SerializeField] private AudioSource _sound;
    [SerializeField] private AudioClip _on;
    [SerializeField] private AudioClip _off;

    private bool _enabled;
    private Color _defaultColor;
    
    private void Start()
    {
        _lampMaterial.EnableKeyword("_EmissionColor");
        _lampMaterial.SetColor("_EmissionColor", _startColor * _emission);
        _defaultColor = _lampMaterial.color;
    }

    public override void OnInputReceived(InputAction.CallbackContext obj)
    {
        _lampMaterial.EnableKeyword("_EmissionColor");
        PlayerCharacter.Instance.SetObjectEnableServer(_lampLightRPC, !_lampLightRPC.activeSelf);
        _lampLight.SetActive(!_lampLight.activeSelf);
        if(!_enabled)
        {
            EnableLight();
            return;
        }
        DisableLight();
    }

    private void EnableLight()
    {
        _enabled = true;
        _sound.clip = _on;
        _sound.Play();
        _lampMaterial.SetColor("_EmissionColor", _defaultColor * 0);
    }

    private void DisableLight()
    {
        _enabled = false;
        _sound.clip = _off;
        _sound.Play();
        _lampMaterial.SetColor("_EmissionColor", _defaultColor * _emission);
    }

    public override void OnEquipStateChanged()
    {
        base.OnEquipStateChanged();
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
    }
}