using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Settings : NetworkBehaviour
{
    [SerializeField] private Canvas _settingsCanvas;

    private PlayerBinds _binds;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!base.IsOwner)
        {
            gameObject.SetActive(false);
            return;
        }
        _binds = PlayerCharacter.Instance.Binds;
        _binds.Character.OpenPanel.started += OnPanelOpened;
    }

    private void OnPanelOpened(InputAction.CallbackContext obj)
    {
        _settingsCanvas.enabled = !_settingsCanvas.enabled;
        if (_settingsCanvas.enabled)
        {
            Time.timeScale = 0;
            //GameCursor.Instance.Show();
            StopAllCoroutines();
        }
        else
        {
            //GameCursor.Instance.Hide();
            Time.timeScale = 1;
        }
    }

    private void OnDisable()
    {
        _binds.Character.OpenPanel.started -= OnPanelOpened;
    }
}