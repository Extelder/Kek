using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class PSXSettings : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _onOffText;

    [SerializeField] private MonoBehaviour[] _psxEffects;

    private PlayerConfig _config;

    private bool _psx;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
            return;
        _config = PlayerConfig.Instance;
        _psx = _config.ConfigData.psx;
        UpdatePsx();
    }

    private void UpdatePsx()
    {
        for (int i = 0; i < _psxEffects.Length; i++)
        {
            _psxEffects[i].enabled = _psx;
        }

        _onOffText.text = _psx ? "On" : "Off";
    }

    public void SwitchPSX()
    {
        _psx = !_psx;
        _config.ConfigData.psx = _psx;
        _config.Save();
        for (int i = 0; i < _psxEffects.Length; i++)
        {
            _psxEffects[i].enabled = _psx;
        }

        UpdatePsx();
    }
}