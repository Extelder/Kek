using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public abstract class GuideStep : NetworkBehaviour
{
    [field :SerializeField] public EnableIndificator EnableIndificator { get; private set; }
    [field :SerializeField] public int StepIndex { get; private set; }
    protected PlayerGuide guide;
    public static event Action<GuideStep> ItemSpawned;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        guide = PlayerGuide.Instance;
        ItemSpawned?.Invoke(this);
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
        guide.SwitchStep();
    }
}