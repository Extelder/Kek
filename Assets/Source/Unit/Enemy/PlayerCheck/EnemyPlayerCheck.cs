using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPlayerCheck : MonoBehaviour
{
    public abstract event Action<PlayerHitBox> PlayerDetected;
    public abstract event Action PlayerLost;
    public abstract void StartChecking();
    public abstract void StopChecking();
}
