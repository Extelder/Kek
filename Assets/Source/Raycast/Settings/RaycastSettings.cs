using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RaycastSettings
{
    [field: SerializeField] public Transform Origin { get; private set; }
    [field: SerializeField] public float MaxDistance { get; private set; }
    [field: SerializeField] public LayerMask LayerMask { get; private set; }
}
