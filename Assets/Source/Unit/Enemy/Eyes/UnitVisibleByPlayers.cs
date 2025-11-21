using UnityEngine;
using UnityEngine.AI;
using FishNet.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class UnitVisibleByPlayers : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    private bool _currentVisible;

    private void Update()
    {
        bool visible = IsVisibleByAnyGameCamera(_skinnedMeshRenderer);

        if (visible != _currentVisible)
        {
            _currentVisible = visible;
            SetPlayerVisibleServer(visible);
            Debug.Log("Visible: " + visible);
        }

        if (_currentVisible)
        {
            _agent.speed = 0f;
        }
        else
        {
            _agent.speed = 2f;
        }
    }

    public static bool IsVisibleByAnyGameCamera(Renderer renderer)
    {
        if (renderer == null)
            return false;

        if (!renderer.isVisible)
            return false;

        foreach (Camera cam in Camera.allCameras)
        {
#if UNITY_EDITOR
            if (cam.cameraType == CameraType.SceneView)
                continue;
#endif

            if (IsRendererVisibleFromCamera(renderer, cam))
                return true;
        }

        return false;
    }

    private static bool IsRendererVisibleFromCamera(Renderer renderer, Camera cam)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerVisibleServer(bool visible)
    {
        SetPlayerVisibleObservers(visible);
    }

    [ObserversRpc]
    public void SetPlayerVisibleObservers(bool visible)
    {
        _currentVisible = visible;
    }
}