using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;


public class PlayerGuide : NetworkBehaviour
{
    public List<GuideStep> _guideSteps = new List<GuideStep>();
    [SerializeField] private int _currentStep;
    private PlayerConfig _config;
    
    public static PlayerGuide Instance { get; private set; }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!Instance)
            Instance = this;
        _config = PlayerConfig.Instance;
        GuideStep.ItemSpawned += OnItemSpawned;
    }

    private void OnItemSpawned(GuideStep step)
    {
        _guideSteps.Add(step);
        SortSteps();
    }

    private void SortSteps()
    {
        _guideSteps = _guideSteps.OrderBy(s => s.StepIndex).ToList();
    }

    public void SwitchStep()
    {
        SwitchStepServer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchStepServer()
    {
        if(_config.ConfigData.guidePassed)
            return;
        _currentStep++;
        Switch();
    }

    private void Update()
    {
        Debug.Log(_currentStep);
    }

    [ObserversRpc(BufferLast = true)]
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
        GuideStep.ItemSpawned -= OnItemSpawned;
    }
}