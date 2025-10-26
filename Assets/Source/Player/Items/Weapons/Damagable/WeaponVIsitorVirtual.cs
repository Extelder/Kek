using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVIsitorVirtual : IWeaponVisitor
{
    public virtual void Visit(TNTThrowable tntThrowable){}
}
