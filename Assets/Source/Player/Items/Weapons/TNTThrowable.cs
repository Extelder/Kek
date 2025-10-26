using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TNTThrowable : NetworkBehaviour
{
    [SerializeField] private float _throwForce;
    [SerializeField] private float _explodeCooldown;
    [SerializeField] private OverlapSettings _overlapSettings;
    private Rigidbody _rigidbody;
    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Throw();
    }

    private void Throw()
    {
        _rigidbody.AddForce(transform.forward * _throwForce, ForceMode.Impulse);
        StartCoroutine(Exploding());
    }

    private IEnumerator Exploding()
    {
        yield return new WaitForSeconds(_explodeCooldown);
        Overlap();
        foreach (var other in _overlapSettings.Colliders)
        {
            if (other == null)
                continue;
            if (other.TryGetComponent<IWeaponVisitor>(out IWeaponVisitor visitor))
            {
                visitor.Visit(this);
                Debug.Log("Visited");
                PlayerCharacter.Instance.DespawnObject(this);
                break;
            }
        }
        PlayerCharacter.Instance.DespawnObject(this);
    }
    
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_overlapSettings.Origin.position, _overlapSettings.SphereRadius);
    }

    private void Overlap()
    {
        _overlapSettings.Colliders = new Collider[10];
        _overlapSettings.Size = Physics.OverlapSphereNonAlloc(
            _overlapSettings.Origin.position,
            _overlapSettings.SphereRadius, _overlapSettings.Colliders,
            _overlapSettings.LayerMask);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
