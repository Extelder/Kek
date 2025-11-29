using System;
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

    public void CallEnable(bool value)
    {
        EnableServer(value);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void EnableServer(bool value)
    {
        if(!IsServer)
            return;
        EnableObserver(value);
    }

    [ObserversRpc(BufferLast = true)]
    private void EnableObserver(bool value)
    {
        _indicatorOff.enabled = value;
        _indicatorOn.enabled = value;   
        Enable(value);
    }

    private void Enable(bool value)
    {
        _indicatorOff.enabled = value;
        _indicatorOn.enabled = value;   
    }

    private void OnDisable()
    {
        if (!base.IsServer)
            return;
        CallEnable(false);
    }
}
