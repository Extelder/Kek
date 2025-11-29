using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;


public class PlayerGuide : NetworkBehaviour
{
    public List<GuideStep> _guideSteps = new List<GuideStep>();
    [SerializeField] private int _currentStep;
    private PlayerConfig _config;

    public override void OnStartClient()
    {
        if (!base.IsServer)
            return;
        base.OnStartClient();
        // _config = PlayerConfig.Instance;
        // if(_config.ConfigData.guidePassed)
        //     return;
        GuideStep.OnSpawned += OnGuideStepSpawned;
    }

    private void OnGuideStepSpawned(GuideStep step)
    {
        _guideSteps.Add(step);
    }

    public void SwitchStep()
    {
        SwitchStepServer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchStepServer()
    {
        if(!IsServer)
            return;
        AddCurrentStep();
        SwitchStepObserver();
    }

    [ObserversRpc]
    public void SwitchStepObserver()
    {
        AddStepServer();
        Switch();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddStepServer()
    {
        if (!IsServer)
            return;
        AddStepObserver();
    }

    [ObserversRpc]
    public void AddStepObserver()
    {
        AddCurrentStep();
    }

    private int AddCurrentStep()
    {
        return _currentStep++;
    }

    private void Update()
    {
        Debug.Log(_currentStep);
    }

    private void Switch()
    {
        if (_currentStep > _guideSteps.Count - 1)
        {
            _config.Save();
            _config.ConfigData.guidePassed = true;
            return;
        }
        _guideSteps[_currentStep].StartStep();
    }

    private void OnDisable()
    {
        if (!base.IsServer)
            return;
        GuideStep.OnSpawned -= OnGuideStepSpawned;
    }
}
