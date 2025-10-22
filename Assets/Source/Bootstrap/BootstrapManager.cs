using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using Steamworks;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    [SerializeField] private string _menuName = "Menu";
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private FishySteamworks.FishySteamworks _fishySteamworks;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public static ulong CurrentLobbyID;

    public static BootstrapManager Instance { get; private set; }

    private void Start()
    {
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            return;
        }
    }

    public static void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
            return;

        CurrentLobbyID = callback.m_ulSteamIDLobby;
        SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress", SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(CurrentLobbyID), "name",
            SteamFriends.GetPersonaName().ToString() + "`s lobby");
        _fishySteamworks.SetClientAddress(SteamUser.GetSteamID().ToString());
        _fishySteamworks.StartConnection(true);

        Debug.Log("Lobby created " + callback.m_eResult.ToString());
    }

    private void OnJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        MainMenuManager.LobbyEntered(SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name"),
            _networkManager.IsServer);

        _fishySteamworks.SetClientAddress(SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "HostAddress"));
        _fishySteamworks.StartConnection(false);
    }


    public void GoToMenu()
    {
        SceneManager.LoadScene(_menuName, LoadSceneMode.Additive);
    }

    public static void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
        CurrentLobbyID = 0;

        Instance._fishySteamworks.StopConnection(false);
        if (Instance._networkManager.IsServer)
            Instance._fishySteamworks.StopConnection(true);
    }

    public static void JoinById(CSteamID steamId)
    {
        Debug.Log("Attempt to join " + steamId.m_SteamID);
        if (SteamMatchmaking.RequestLobbyData(steamId))
        {
            SteamMatchmaking.JoinLobby(steamId);
        }
        else
        {
            Debug.Log("Failed to join");
        }
    }
}