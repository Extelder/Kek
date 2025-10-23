using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterVolumeSettings : SoundVolumeSettings
{
    public override void StartVirtual()
    {
        base.StartVirtual();
        Slider.value = Config.ConfigData.masterVolume;
    }

    public override void ChangeSoundVolume(float value)
    {
        Config.Save();
        base.ChangeSoundVolume(value);
        Config.ConfigData.masterVolume = value;
    }
}
