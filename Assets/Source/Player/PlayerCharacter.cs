using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [field: SerializeField] public Transform DropPoint { get; private set; }
    [field: SerializeField] public Transform FingerLookAtPoint { get; private set; }
    [field: SerializeField] public Transform TargetPoint { get; private set; }
    [field: SerializeField] public Transform CartPoint { get; private set; }
    [field: SerializeField] public Transform CameraTransform { get; private set; }
    [field: SerializeField] public GameObject Hands { get; private set; }
    [field: SerializeField] public GameObject TransitHands { get; private set; }
    [field: SerializeField] public Rigidbody Rigidbody;
    [field: SerializeField] public PlayerBinds Binds;
    [field: SerializeField] public Transform PlayerTransform;
    [field: SerializeField] public GameObject[] _thirdPerson;
    [field: SerializeField] public GameObject _inventory;
    [field: SerializeField] public PlayerInventory PlayerInventory { get; private set; }
    [field: SerializeField] public PlayerHatsEquip PlayerHatsEquip { get; private set; }
    [field: SerializeField] public PlayerHealth PlayerHealth { get; private set; }
    [field: SerializeField] public PlayerWallet Wallet { get; private set; }

    public float Distance { get; set; }

    public static PlayerCharacter Instance { get; private set; }

    public event Action ClientStarted;

    public List<PlayerCharacter> Characters = new List<PlayerCharacter>();

    [ServerRpc(RequireOwnership = false)]
    public void ServerSpawnObject(GameObject spawnedObject, Vector3 position, Quaternion rotation)
    {
        GameObject instance = Instantiate(spawnedObject, position, rotation);
        ServerManager.Spawn(instance);
    }


    [ServerRpc(RequireOwnership = false)]
    public void DespawnObject(NetworkBehaviour spawnedObject)
    {
        spawnedObject.Despawn();
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            Binds = InputManager.inputActions;

            Binds.Enable();

            for (int i = 0; i < _thirdPerson.Length; i++)
            {
                _thirdPerson[i].SetActive(false);
            }

            Instance = this;
        }
        else
        {
            _inventory.SetActive(false);
        }

        Instance.Characters.Add(this);
        ClientStarted?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetObjectEnableServer(GameObject needObject, bool enabled)
    {
        SetObjectEnableObserver(needObject, enabled);
    }

    [ObserversRpc]
    public void SetObjectEnableObserver(GameObject gameObject, bool enabled)
    {
        gameObject.SetActive(enabled);
        Debug.LogError(gameObject);
    }


    private void OnDisable()
    {
        if (!base.IsOwner)
            return;
        Binds?.Dispose();
        Binds?.Disable();
    }
}