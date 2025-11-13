using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGProjectile : MonoBehaviour
{
    [field :SerializeField] public float Damage { get; private set; }
    [SerializeField] private OverlapSettings _overlapSettings;
    [SerializeField] private int _collidersSize;
    private Collider[] _colliders;
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent<RPGProjectile>(out RPGProjectile rpgProjectile))
            return;
        Explode();
    }

    private void Explode()
    {
        _colliders = new Collider[_collidersSize];
        Overlap();
        foreach (var other in _colliders)
        {
            
            if (!other)
                continue;
            if (other.TryGetComponent<IWeaponVisitor>(out IWeaponVisitor visitor))
            {
                visitor.Visit(this);
            }

            if (other.TryGetComponent<PlayerHitBox>(out PlayerHitBox playerHitBox))
            {
                Debug.Log("PLAYERHITBOX");
                playerHitBox.TakeDamage(Damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_overlapSettings.Origin.position,
            _overlapSettings.SphereRadius);
    }

    private void Overlap()
    {
        _overlapSettings.Size = Physics.OverlapSphereNonAlloc(_overlapSettings.Origin.position,
            _overlapSettings.SphereRadius, _colliders, _overlapSettings.LayerMask);
    }
}
