using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct OverlapSettings
{
    [field: SerializeField] public Transform Origin { get; private set; }
    [field: SerializeField] public LayerMask LayerMask { get; private set; }
    [field: SerializeField] public float SphereRadius { get; private set; }
    [field: SerializeField] public int Size { get; set; }
    public Collider[] Colliders { get; set; }
}
