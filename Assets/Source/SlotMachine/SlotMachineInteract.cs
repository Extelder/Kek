using System.Collections;
using UnityEngine;

public class SlotMachineInteract : Item
{
    [SerializeField] private SlotMachineSpin _slotMachineSpin;

    public override void Interact()
    {
        if (!_slotMachineSpin.spinning)
        {
            _slotMachineSpin.StartSpin();
        }
    }
}