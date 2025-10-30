using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public abstract class EnemyPlayerCheck : NetworkBehaviour
{
    public abstract event Action<PlayerHitBox> PlayerDetected;
    public abstract event Action PlayerLost;
    public abstract void StartChecking();
    public abstract void StopChecking();
}
