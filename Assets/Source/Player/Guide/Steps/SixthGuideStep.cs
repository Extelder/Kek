using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixthGuideStep : GuideStep
{
    [SerializeField] private InteractItem _interactItem;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _interactItem.Interacted += OnInteracted;
    }

    private void OnInteracted()
    {
        OnStepEnded();
    }

    private void OnDisable()
    {
        _interactItem.Interacted += OnInteracted;
    }
}
