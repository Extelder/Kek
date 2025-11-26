using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public abstract class GuideStep : NetworkBehaviour
{
    [field: SerializeField] public int Step { get; private set; }
    
    public static event Action<GuideStep> OnSpawned;

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnSpawned?.Invoke(this);
    }

    public abstract void StartStep();

    public virtual void StopStep()
    {
        PlayerCharacter.Instance.PlayerGuide.SwitchStep();
    }
}
