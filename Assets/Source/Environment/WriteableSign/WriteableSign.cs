using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class WriteableSign : NetworkBehaviour
{
    private TMP_InputField _inputField;
    private PlayerCharacter _character;

    public void OnInteracted(TMP_InputField inputField)
    {
        _character = PlayerCharacter.Instance;
        _inputField = inputField;
        _character.Rigidbody.isKinematic = true;
        _character.SetCinemachienCameraValueZero();
        _character.CameraHeadBob.EnableAnimator(false);
        _character.Binds.Character.Apply.started += OnApllied;
    }

    private void OnApllied(InputAction.CallbackContext obj)
    {
        _inputField.DeactivateInputField();
        _character.SetCinemachineCameraDefaultValue();
        _character.CameraHeadBob.EnableAnimator(true);
        _character.Rigidbody.isKinematic = false;
        SetString(_inputField.text);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SetString(string text)
    {
        if (!IsServer)
            return;

        NotifyValueChanged(text);
        Debug.Log(text + "Server");
    }

    [ObserversRpc(BufferLast = true)]
    public void NotifyValueChanged(string text)
    {
        _inputField.text = text;
        Debug.Log(text + "Observer");
    }

    private void OnDisable()
    {
        if(_character == null)
            return;
        _character.Binds.Character.Apply.started -= OnApllied;
    }
}
