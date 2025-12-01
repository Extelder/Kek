using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FootStep
{
    rock, iron
}
public class GroundChecker : MonoBehaviour
{
    public FootStep Foot;
    [SerializeField] private RaycastSettings _raycastSettings;
    [SerializeField] private Rigidbody rb;
    public bool Detected { get; private set; }
    private void FixedUpdate()
    {
        if (Physics.Raycast(_raycastSettings.Origin.position, -_raycastSettings.Origin.up, out RaycastHit hit,
            _raycastSettings.MaxDistance,
            _raycastSettings.LayerMask))
        {
            Detected = true;
            rb.AddForce(Vector3.down * 40f, ForceMode.Acceleration);
            if (hit.collider.TryGetComponent(out BoerCheck boerCheck))
            {
                Foot = FootStep.iron;
            }
            else
            {
                Foot = FootStep.rock;
            }
        }
        else
        {
            Detected = false;
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(_raycastSettings.Origin.position, -_raycastSettings.Origin.up * _raycastSettings.MaxDistance);
    }
}