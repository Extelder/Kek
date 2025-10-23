using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class ResolutionScreenSettings : NetworkBehaviour
{
    [SerializeField] private TMP_Dropdown _resolutionDropdown;

    private Resolution[] _resolutions;
    private List<Resolution> _filteredResolutions;
    private float _currentRefrashRate;
    private int _currentResolutionIndex = 0;

    private PlayerConfig _config;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!base.IsOwner)
            return;
        _config = PlayerConfig.Instance;
        Screen.SetResolution(_config.ConfigData.width, _config.ConfigData.height, _config.ConfigData.fullScreen);
        SetResolutionReady();
    }

    private void SetResolutionReady()
    {
        _resolutions = Screen.resolutions;
        _filteredResolutions = new List<Resolution>();

        _resolutionDropdown.ClearOptions();
        _currentRefrashRate = Screen.currentResolution.refreshRate;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            if (_resolutions[i].refreshRate == _currentRefrashRate)
            {
                _filteredResolutions.Add(_resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < _filteredResolutions.Count; i++)
        {
            string resolutionOption = _filteredResolutions[i].width + "x" + _filteredResolutions[i].height + " " +
                                      _filteredResolutions[i].refreshRate + "Hz";
            options.Add(resolutionOption);
            if (_filteredResolutions[i].width == Screen.width && _filteredResolutions[i].height == Screen.height)
            {
                _currentResolutionIndex = i;
            }
        }

        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = _currentResolutionIndex;
        _resolutionDropdown.RefreshShownValue();
    }

    public void ChangeResolutions(int resolutionIndex)
    {
        Resolution resolution = _filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
        _config.ConfigData.height = resolution.height;
        _config.ConfigData.width = resolution.width;
        _config.Save();
    }
}
