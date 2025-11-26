using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstGuideStep : GuideStep
{
    [SerializeField] private EnableIndificator _enableIndificator;
    
    public override void OnStartClient()
    {
        if(!base.IsServer)
            return;
        base.OnStartClient();
        StartStep();
    }

    public override void StartStep()
    {
        _enableIndificator.Enable();
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(10);
        StopStep();
    }
}
