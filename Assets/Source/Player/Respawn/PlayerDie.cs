using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

public class PlayerDie : NetworkBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private GameObject[] _offObjectsWhenDie;

    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerInteract _interact;

    public void Die()
    {
        DieServer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DieServer()
    {
        DieObserver();
    }

    [ObserversRpc]
    public void DieObserver()
    {
        for (int i = 0; i < _offObjectsWhenDie.Length; i++)
        {
            _offObjectsWhenDie[i].SetActive(false);
        }

        _collider.enabled = false;
        PlayerCharacter.Instance.Rigidbody.useGravity = false;
        _movement.CanFly = true;
        _interact.enabled = false;
    }
}