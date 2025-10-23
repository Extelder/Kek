using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig : MonoBehaviour
{
    public static PlayerConfig Instance { get; private set; }
    public PlayerConfigData ConfigData;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            ConfigData = new PlayerConfigData();
            ConfigData = FreeDoomSettingsSaveLoad.Load();
            return;
        }
        Debug.LogError("Theres one more PlayerConfig");
    }

    public void Save()
    {
        FreeDoomSettingsSaveLoad.Save(ConfigData);
    }
}
