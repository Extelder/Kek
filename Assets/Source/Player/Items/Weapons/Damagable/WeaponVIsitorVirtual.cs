using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVIsitorVirtual : IWeaponVisitor
{
    public void Visit(RPGProjectile rpgProjectile)
    {
    }

    public virtual void Visit(TNTThrowable tntThrowable)
    {
    }

    public virtual void Visit(Pickaxe pickaxe, RaycastHit hit)
    {
    }

    public virtual void Visit(Drill drill, RaycastHit hit)
    {
    }
}