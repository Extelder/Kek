using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private Transform _camera;

    private void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, _camera.localEulerAngles.y, transform.localEulerAngles.z); 
    }
}
