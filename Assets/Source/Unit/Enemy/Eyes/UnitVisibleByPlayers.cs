using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UnitVisibleByPlayers : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    private bool _currentVisible;

    void Update()
    {
        bool visible = IsVisibleByAnyGameCamera(_skinnedMeshRenderer);

        if (visible != _currentVisible)
        {
            _currentVisible = visible;
            SetPlayerVisibleServer(visible);
            Debug.Log("Visible: " + visible);
        }
    }

    // Проверка видимости во ВСЕХ игровый камерах
    public static bool IsVisibleByAnyGameCamera(Renderer renderer)
    {
        if (renderer == null)
            return false;

        if (!renderer.isVisible) 
            return false;

        foreach (Camera cam in Camera.allCameras)
        {
#if UNITY_EDITOR
            // Пропускаем SceneView
            if (cam.cameraType == CameraType.SceneView)
                continue;
#endif

            if (IsRendererVisibleFromCamera(renderer, cam))
                return true;
        }

        return false;
    }

    // Фрустум-проверка
    private static bool IsRendererVisibleFromCamera(Renderer renderer, Camera cam)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    public void SetPlayerVisibleServer(bool visible)
    {
        SetPlayerVisibleObservers(visible);
    }

    public void SetPlayerVisibleObservers(bool visible)
    {
        _currentVisible = visible;
    }
}