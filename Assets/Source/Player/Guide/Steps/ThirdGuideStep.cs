using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdGuideStep : GuideStep
{
    [SerializeField] private EnableIndificator _enableIndificator;
    [SerializeField] private CollectTrigger _collectTrigger;

    public override void OnStartClient()
    {
        if (!base.IsServer)
            return;
        base.OnStartClient();
        _collectTrigger.ItemAte += OnItemAte;
    }

    public override void StartStep()
    {
        _enableIndificator.CallEnable(true);
        Debug.Log("STEP CHANGED");
    }

    private void OnItemAte()
    {
        StopStep();
        _enableIndificator.CallEnable(false);
    }

    private void OnDisable()
    {
        _collectTrigger.ItemAte -= OnItemAte;
    }
}