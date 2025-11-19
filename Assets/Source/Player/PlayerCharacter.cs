using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FishNet.Object;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public CameraHeadBob CameraHeadBob { get; private set; }
    [field: SerializeField] public PlayerHealth PlayerHealth { get; private set; }
    [field: SerializeField] public Transform DropPoint { get; private set; }
    [field: SerializeField] public Transform FingerLookAtPoint { get; private set; }
    [field: SerializeField] public Transform TargetPoint { get; private set; }
    [field: SerializeField] public Transform CartPoint { get; private set; }
    [field: SerializeField] public Transform CameraTransform { get; private set; }
    [field: SerializeField] public Rigidbody Rigidbody;
    [field: SerializeField] public PlayerBinds Binds;
    [field: SerializeField] public Transform PlayerTransform;
    [field: SerializeField] public GameObject[] _thirdPerson;
    [field: SerializeField] public GameObject _inventory;
    [field: SerializeField] public PlayerInventory PlayerInventory { get; private set; }
    [field: SerializeField] public PlayerHatsEquip PlayerHatsEquip { get; private set; }
    [field: SerializeField] public PlayerWallet PlayerWallet { get; private set; }
    [field: SerializeField] public PlayerMovement PlayerMovement { get; private set; }
    [field: SerializeField] public PlayerHitBox PlayerHitBox { get; private set; }

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private GameObject _hands;
    [SerializeField] private GameObject _transitionHands;


    [SerializeField] private GameObject _poolsPrefab;
    private CinemachinePOV _cinemachinePov;
    private PlayerConfig _config;

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

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            if (IsServer)
            {
                GameObject instance = Instantiate(_poolsPrefab, transform.position, Quaternion.identity);
                ServerManager.Spawn(instance);
                Debug.Log("SPawned" + GetInstanceID());
            }

            Binds = InputManager.inputActions;

            Binds.Enable();

            for (int i = 0; i < _thirdPerson.Length; i++)
            {
                _thirdPerson[i].SetActive(false);
            }

            _cinemachinePov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
            _config = PlayerConfig.Instance;

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

    public void SetCinemachienCameraValueZero()
    {
        _cinemachinePov.m_HorizontalAxis.m_MaxSpeed = 0;
        _cinemachinePov.m_VerticalAxis.m_MaxSpeed = 0;
    }

    public void SetCinemachineCameraDefaultValue()
    {
        _cinemachinePov.m_HorizontalAxis.m_MaxSpeed = _config.ConfigData.lookSensitivity;
        _cinemachinePov.m_VerticalAxis.m_MaxSpeed = _config.ConfigData.lookSensitivity;
    }

    public void SwitchHands()
    {
        _hands.SetActive(!_hands.activeSelf);
        _transitionHands.SetActive(!_transitionHands.activeSelf);
    }

    private void OnDisable()
    {
        if (!base.IsOwner)
            return;
        Binds?.Dispose();
        Binds?.Disable();
    }
}