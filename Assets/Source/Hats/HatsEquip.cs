    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatsEquip : Item
{
    [SerializeField] private int _id;
    public override void Interact()
    {
        PlayerCharacter.Instance.PlayerHatsEquip.ActivateHat(_id);
    }
}
