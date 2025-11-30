using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class SecondGuideStep : GuideStep
{
    [SerializeField] private Ore _ore;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _ore.Destroyed += OnDestroyed;
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