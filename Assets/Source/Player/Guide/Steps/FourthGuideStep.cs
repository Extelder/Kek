using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourthGuideStep : GuideStep
{
    [SerializeField] private ControlPanelAnimator _controlPanelAnimator;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _controlPanelAnimator.Activated += OnActivated;
    }

    private void OnActivated()
    {
        OnStepEnded();
    }

    private void OnDisable()
    {
        _controlPanelAnimator.Activated -= OnActivated;
    }
}
