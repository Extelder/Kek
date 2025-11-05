using System;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

public class PlayerDie : NetworkBehaviour
{
    [SerializeField] private Transform _deadBodySourceBone;

    [SerializeField] private DeadPlayer _deadPlayer;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Die();
        }
    }

    [ObserversRpc]
    public void DieObserver()
    {
        
        DeadPlayer player =
            Instantiate(_deadPlayer, _deadBodySourceBone.transform.position, _deadBodySourceBone.rotation)
                .GetComponent<DeadPlayer>();
        ServerManager.Spawn(player.gameObject);
        _deadPlayer.CopyBones(_deadBodySourceBone);
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