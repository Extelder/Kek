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
        PlayerTransitionHands.BackedToDefault += OnBackedToDefault;
    }

    private void OnTargetDestinated()
    {
        _canvas.worldCamera = PlayerCharacter.Instance.Camera;
    }

    private void OnBackedToDefault()
    {
        _canvas.worldCamera = null;
    }

    private void OnDisable()
    {
        PlayerTransitionHands.TargetDestinated -= OnTargetDestinated;
        PlayerTransitionHands.BackedToDefault -= OnBackedToDefault;
    }
}
