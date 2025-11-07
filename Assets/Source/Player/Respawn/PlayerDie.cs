using System;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

public class PlayerDie : NetworkBehaviour
{
    [SerializeField] private GameObject _deadLight;
    [SerializeField] private Transform _deadBodySourceBone;

    [SerializeField] private DeadPlayer _deadPlayer;
    [SerializeField] private GameObject[] _offObjectsWhenDie;

    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerInteract _interact;

    public void Die()
    {
        if (IsOwner)
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
        _deadLight.SetActive(true);
        for (int i = 0; i < _offObjectsWhenDie.Length; i++)
        {
            if (_offObjectsWhenDie[i] == null)
                continue;
            _offObjectsWhenDie[i].SetActive(false);
        }

        PlayerCharacter.Instance.Rigidbody.useGravity = false;
        _movement.CanFly = true;
        _interact.enabled = false;

        gameObject.layer = LayerMask.NameToLayer("PlayerWithoutPlayerCollision");


        DeadPlayer player =
            Instantiate(_deadPlayer, transform.position, transform.rotation)
                .GetComponent<DeadPlayer>();
        player.CopyBones(_deadBodySourceBone);
    }
}