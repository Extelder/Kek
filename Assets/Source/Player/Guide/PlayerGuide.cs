using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;


public class PlayerGuide : NetworkBehaviour
{
    public List<GuideStep> _guideSteps = new List<GuideStep>();
    [SerializeField] private int _currentStep;
    
    public override void OnStartClient()
    {
        if (!base.IsServer)
            return;
        base.OnStartClient();
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
        _currentStep++;
        int currentStep = _currentStep;
        AddStepObserver(currentStep);
    }

    [ObserversRpc]
    public void AddStepObserver(int currentStep)
    {
        _currentStep = currentStep;
    }

    private void Update()
    {
        Debug.Log(_currentStep);
    }

    private void Switch()
    {
        if (_currentStep > _guideSteps.Count - 1)
            return;
        Debug.Log("STEP CHANGED " + _currentStep);
        _guideSteps[_currentStep].StartStep();
    }

    private void OnDisable()
    {
        if (!base.IsServer)
            return;
        GuideStep.OnSpawned -= OnGuideStepSpawned;
    }
}
