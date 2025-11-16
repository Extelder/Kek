using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class OreQuota : NetworkBehaviour
{
    [field: SerializeField] public InteractItem InteractItem { get; private set; }
}