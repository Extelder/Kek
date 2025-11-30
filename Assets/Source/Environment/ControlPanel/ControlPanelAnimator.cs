using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelAnimator : MonoBehaviour
{
    [SerializeField] private PlayersInChecker _playersInChecker;
    public event Action Activated;

    public void Activate()
    {
        _playersInChecker.gameObject.SetActive(true);
        Activated?.Invoke();
    }

    public void Deactive()
    {
        _playersInChecker.gameObject.SetActive(false);
    }
}