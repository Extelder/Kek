using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;


public class PlayerGuide : NetworkBehaviour
{
    public List<GuideStep> _guideSteps = new List<GuideStep>();
    private int _lastSortedStep;
    private int _currentStep;
    private int _stepNumber;
    
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

    [ObserversRpc(BufferLast = true)]
    public void SwitchStepObserver()
    {
        Switch();
    }

    private void Switch()
    {
        _currentStep++;
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
