using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValveInteractable : Item
{
    [SerializeField] private ValveSpin _valve;

    public override void Interact()
    {
        _valve.Press();
    }

    public override void InteractCancelled()
    {
        _valve.UnPress();
    }
}
