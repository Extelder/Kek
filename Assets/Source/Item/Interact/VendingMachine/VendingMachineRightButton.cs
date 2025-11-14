using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class VendingMachineRightButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private VendingMachinePanelsSwitcher _vendingMachinePanelsSwitcher;
    public void OnPointerDown(PointerEventData eventData)
    {
        _vendingMachinePanelsSwitcher.NextPage();
    }
}
