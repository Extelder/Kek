using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public abstract class GuideStep : NetworkBehaviour
{
    [field :SerializeField] public EnableIndificator EnableIndificator { get; private set; }
    [SerializeField] private PlayerGuide _guide;
    
    public static event Action<GuideStep> OnSpawned;

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnSpawned?.Invoke(this);
    }

    public virtual void StartStep()
    {
        EnableIndificator.CallEnable(true);
    }

    public virtual void OnStepEnded()
    {
        StopStep();
        EnableIndificator.CallEnable(false);
    }

    public virtual void StopStep()
    {
        _guide.SwitchStep();
    }
}