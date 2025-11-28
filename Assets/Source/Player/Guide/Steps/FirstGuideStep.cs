using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstGuideStep : GuideStep
{
    [SerializeField] private EnableIndificator _enableIndificator;
    [SerializeField] private InteractItem _interactItem;

    public override void OnStartClient()
    {
        if(!base.IsServer)
            return;
        base.OnStartClient();
        StartStep();
        _interactItem.Interacted += OnInteracted;
    }

    public override void StartStep()
    {
        _enableIndificator.CallEnable(true);
    }

    private void OnInteracted()
    {
        StopStep();
    }

    private void OnDisable()
    {
        _interactItem.Interacted -= OnInteracted;
    }
}
