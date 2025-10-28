using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : State
{
    [field: SerializeField] public EnemyAnimator Animator { get; private set; }
    public abstract override void Enter();
}
