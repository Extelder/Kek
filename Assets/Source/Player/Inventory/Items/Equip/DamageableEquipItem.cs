using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class DamageableEquipItem : EquipItem
{
    [field: SerializeField] public float Damage { get; private set; }
    
    public abstract override void OnInputReceived(InputAction.CallbackContext obj);

    public abstract override void OnInputCanceled(InputAction.CallbackContext obj);
}
