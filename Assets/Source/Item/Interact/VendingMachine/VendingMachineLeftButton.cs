using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingMachineLeftButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private VendingMachinePanelsSwitcher _vendingMachinePanelsSwitcher;
    public void OnPointerDown(PointerEventData eventData)
    {
        _vendingMachinePanelsSwitcher.PreviousPage();
    }
}
