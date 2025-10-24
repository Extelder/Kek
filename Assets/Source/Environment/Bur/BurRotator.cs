using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurRotator : MonoBehaviour
{
    [SerializeField] private GameObject _bur;
    private void Update()
    {
        _bur.transform.Rotate(transform.right,Space.Self);
    }
}
