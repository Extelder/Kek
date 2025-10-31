using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponVisitor
{
    public void Visit(TNTThrowable tntThrowable);
    public void Visit(Pickaxe pickaxe, RaycastHit hit);
    public void Visit(Drill drill, RaycastHit hit);
}