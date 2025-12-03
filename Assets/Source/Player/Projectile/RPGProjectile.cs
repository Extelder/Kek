using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class RPGProjectile : NetworkBehaviour
{
    [field :SerializeField] public float Damage { get; private set; }
    [SerializeField] private OverlapSettings _overlapSettings;
    [SerializeField] private int _collidersSize;
    [SerializeField] private float _cooldowmToDespawn;
    [SerializeField] private ParticleSystem _particle;
    private Collider[] _colliders;
    private bool _canExplode = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<RPGProjectile>(out RPGProjectile rpgProjectile))
            return;
        if (other == PlayerCharacter.Instance.Collider)
            return;
        if (!_canExplode)
            return;
        Explode();
    }

    private void Explode()
    {
        _particle.Play();
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
                playerHitBox.TakeDamage(Damage);
            }
        }

        ServerDespawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerDespawn()
    {
        ObseverDespawn();
    }

    [ObserversRpc]
    public void ObseverDespawn()
    {
        StartCoroutine(Despawning());
    }

    private IEnumerator Despawning()
    {
        yield return new WaitForSeconds(_cooldowmToDespawn);
        _canExplode = true;
        Despawn();
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
