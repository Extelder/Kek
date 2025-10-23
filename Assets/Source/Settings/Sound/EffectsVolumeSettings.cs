using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsVolumeSettings : SoundVolumeSettings
{
    public override void StartVirtual()
    {
        base.StartVirtual();
        Slider.value = Config.ConfigData.effectsVolume;
    }

    public override void ChangeSoundVolume(float value)
    {
        Config.Save();
        base.ChangeSoundVolume(value);
        Config.ConfigData.effectsVolume = value;
    }
}
