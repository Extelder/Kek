using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Buyable")]
public class BuyableItemData : ScriptableObject
{
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}
