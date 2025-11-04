using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasCamera : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;

    private void OnEnable()
    {
        PlayerTransitionHands.TargetDestinated += OnTargetDestinated;
    }

    private void OnTargetDestinated()
    {
        _canvas.worldCamera = PlayerCharacter.Instance.Camera;
    }

    private void OnDisable()
    {
        PlayerTransitionHands.TargetDestinated -= OnTargetDestinated;
    }
}
