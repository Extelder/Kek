using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FithGuideStep : GuideStep
{
    [SerializeField] private PlayersInChecker _playersInChecker;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _playersInChecker.AllPlayerInAction += OnAllPlayersInAction;
    }

    private void OnAllPlayersInAction()
    {
        if (!IsServer)
                return;
        OnStepEnded();
    }

    private void OnDisable()
    {
        _playersInChecker.AllPlayerInAction -= OnAllPlayersInAction;
    }
}
