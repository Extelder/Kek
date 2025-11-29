using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstGuideStep : GuideStep
{
    [SerializeField] private EnableIndificator _enableIndificator;
    [SerializeField] private InteractItem _interactItem;
    private PlayerConfig _config;

    public override void OnStartClient()
    {
      
        base.OnStartClient();
        // _config = PlayerConfig.Instance;
        // if (_config.ConfigData.guidePassed)
        //     return;
        StartStep();
        _interactItem.Interacted += OnInteracted;
    }

    public override void StartStep()
    {
        _enableIndificator.CallEnable(true);
    }

    private void OnInteracted()
    {
        Debug.Log("YAICA");
        StopStep();
    }

    private void OnDisable()
    {
       
        _interactItem.Interacted -= OnInteracted;
    }
}