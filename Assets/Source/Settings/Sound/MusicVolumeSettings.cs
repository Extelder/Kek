using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVolumeSettings : SoundVolumeSettings
{
    public override void StartVirtual()
    {
        base.StartVirtual();
        Slider.value = Config.ConfigData.musicVolume;
    }

    public override void ChangeSoundVolume(float value)
    {
        Config.Save();
        base.ChangeSoundVolume(value);
        Config.ConfigData.musicVolume = value;
    }
}
