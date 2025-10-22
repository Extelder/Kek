using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _menuScreen, _lobbyScreen;

    [SerializeField] private TMP_InputField _lobbyInput;

    [SerializeField] private TextMeshProUGUI _lobbyTitle, _lobbyIDTExt;
    [SerializeField] private Button _startGameButton;
    public static MainMenuManager Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    public void StartGame()
    {
        BootstrapNetworkManager.ChangeNetowrkScene("SampleScene", new[] {"Menu"});
    }

    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        CloseAllScreens();
        _menuScreen.SetActive(true);
    }

    public static void LobbyEntered(string lobbyName, bool isHost)
    {
        Instance._lobbyTitle.text = lobbyName;
        Instance._startGameButton.gameObject.SetActive(isHost);
        Instance._lobbyIDTExt.text = BootstrapManager.CurrentLobbyID.ToString();
        Instance.OpenLobby();
    }

    public void OpenLobby()
    {
        CloseAllScreens();
        _lobbyScreen.SetActive(true);
    }

    public void JoinLobby()
    {
        CSteamID steamId = new CSteamID(Convert.ToUInt64(_lobbyInput.text));
        BootstrapManager.JoinById(steamId);
    }

    public void LeaveLobby()
    {
        BootstrapManager.LeaveLobby();
        OpenMainMenu();
    }

    public void CloseAllScreens()
    {
        _menuScreen.SetActive(false);
        _lobbyScreen.SetActive(false);
    }

    public void CreateLobby()
    {
        BootstrapManager.CreateLobby();
    }
}