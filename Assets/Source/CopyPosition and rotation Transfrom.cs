using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPositionandrotationTransfrom : MonoBehaviour
{
    [SerializeField] private Transform _target;
    
    private void Update()
    {
        _target.position = transform.position;
        _target.rotation = transform.rotation;
    }
}
