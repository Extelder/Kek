using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPotion : Item
{
    [SerializeField] private PotionEffects _script;
    public override void Interact()
    {
        _script.Drink();
    }
}