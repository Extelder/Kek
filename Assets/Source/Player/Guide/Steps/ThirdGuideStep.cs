using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdGuideStep : GuideStep
{
    [SerializeField] private CollectTrigger _collectTrigger;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _collectTrigger.ItemAte += OnItemAte;
    }

    private void OnItemAte()
    {
        if (!IsServer)
            return;
        OnStepEnded();
    }

    private void OnDisable()
    {
        _collectTrigger.ItemAte -= OnItemAte;
    }
}