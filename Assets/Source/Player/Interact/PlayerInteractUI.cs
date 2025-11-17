using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject _interactButton;
    [SerializeField] private PlayerInteract _interact;

    private void OnEnable()
    {
        _interact.DetectedStateChanged += OnDetectedStateChanged;
    }

    private void OnDetectedStateChanged(bool detected)
    {
        _interactButton.SetActive(detected);
    }

    private void OnDisable()
    {
        _interact.DetectedStateChanged -= OnDetectedStateChanged;
    }
}