using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class HitBox : NetworkBehaviour, IWeaponVisitor
{
    public event Action PickAxeHitted;
    public event Action DrillHitted;
    
    public virtual void Visit(RPGProjectile rpgProjectile)
    {
    }

    public virtual void Visit(TNTThrowable tntThrowable)
    {
    }

    public virtual void Visit(Pickaxe pickaxe, RaycastHit hit)
    {
        PickAxeHitted?.Invoke();
        Debug.Log("PICKAXE");
    }

    public virtual void Visit(Drill drill, RaycastHit hit)
    {
        DrillHitted?.Invoke();
    }
}
