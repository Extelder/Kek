using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using HUDIndicator;
using UnityEngine;

[RequireComponent(typeof(IndicatorOnScreen), typeof(IndicatorOffScreen))]
public class EnableIndificator : NetworkBehaviour
{
    [SerializeField] private IndicatorOffScreen _indicatorOff;
    [SerializeField] private IndicatorOnScreen _indicatorOn;
    [SerializeField] private bool _enableOnClientStarted;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsServer || !_enableOnClientStarted)
            return;
        Enable();
    }

    public void Enable()
    {
        _indicatorOff.enabled = true;
        _indicatorOn.enabled = true;
    }
}
