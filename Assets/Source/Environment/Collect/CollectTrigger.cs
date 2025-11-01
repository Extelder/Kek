using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectTrigger : MonoBehaviour
{
    public event Action ItemDelivered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<InteractItem>(out InteractItem interactItem))
        {
            if (interactItem.Item is PickUpableItem)
            {
                Debug.Log("Delivered");
                interactItem.Despawn();
                ItemDelivered?.Invoke();
                return;
            }
        }
    }
}
