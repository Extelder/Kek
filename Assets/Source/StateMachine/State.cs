using FishNet.Object;
using UnityEngine;

public abstract class State : NetworkBehaviour
{
   public bool CanChanged = true;

   public abstract void Enter();

   public virtual void Exit() {}
}
