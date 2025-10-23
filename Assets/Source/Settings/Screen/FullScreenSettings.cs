using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class FullScreenSettings : NetworkBehaviour
{
    private PlayerConfig _config;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!base.IsOwner)
            return;
        _config = PlayerConfig.Instance;
        Screen.fullScreen = _config.ConfigData.fullScreen;
    }

    public void SwitchFullScreenSettings()
    {
        Screen.fullScreen = !Screen.fullScreen;
        _config.ConfigData.fullScreen = Screen.fullScreen;
        _config.Save();
    }
}
