using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class SecondGuideStep : GuideStep
{
    [SerializeField] private EnableIndificator _enableIndificator;
    [SerializeField] private Ore _ore;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _ore.Destroyed += OnDestroyed;
    }

    public override void StartStep()
    {
        _enableIndificator.CallEnable(true);
        Debug.Log("STEP CHANGED");
    }

    private void OnDestroyed()
    {
        if (!IsServer)
            return;
        StopStep();
    }

    private void OnDisable()
    {
        _ore.Destroyed -= OnDestroyed;
    }
}