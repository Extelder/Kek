using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using HUDIndicator;
using UnityEngine;

public class IndicatorOnlinePlayerCameraSetter : NetworkBehaviour
{
    [SerializeField] private IndicatorRenderer _indicatorRenderer;

    public override void OnStartClient()
    {
        base.OnStartClient();
        _indicatorRenderer.camera = PlayerCharacter.Instance.Camera;
    }
}