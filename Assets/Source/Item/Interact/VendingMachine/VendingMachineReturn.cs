using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class VendingMachineReturn : MonoBehaviour, IPointerDownHandler
{
    public static event Action Returned;
    public void OnPointerDown(PointerEventData eventData)
    {
        Return();
    }

    private void Return()
    {
        Returned?.Invoke();
    }
}
