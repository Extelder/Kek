using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

public class BootstrapNetworkManager : NetworkBehaviour
{
    private static BootstrapNetworkManager _instance;

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            return;
        }

        Debug.LogError("One more BootstrapNetworkManager");
    }

    public static void ChangeNetowrkScene(string sceneName, string[] sceneToClose)
    {
        _instance.CloseScenes(sceneToClose);
        
        SceneLoadData sld = new SceneLoadData(sceneName);
        NetworkConnection[] conns = _instance.ServerManager.Clients.Values.ToArray();
        _instance.SceneManager.LoadConnectionScenes(conns, sld);
    }

    [ServerRpc(RequireOwnership = false)]
    void CloseScenes(string[] scenes)
    {
        CloseScenesObserver(scenes);
    }

    [ObserversRpc]
    void CloseScenesObserver(string[] scenes)
    {
        foreach (var sceneName in scenes)
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}