using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettings : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] private Slider _sensetivitySlider;
    
    private CinemachinePOV _cinemachinePOV;
    private PlayerConfig _config;

    private void Start()
    {
        _config = PlayerConfig.Instance;
            
        _cinemachinePOV = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        _cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = _config.ConfigData.lookSensitivity;
        _cinemachinePOV.m_VerticalAxis.m_MaxSpeed = _config.ConfigData.lookSensitivity;
        _sensetivitySlider.value = _config.ConfigData.lookSensitivity;
    }

    public void SetSensitivity(float value)
    {
        _cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = value;
        _cinemachinePOV.m_VerticalAxis.m_MaxSpeed = value;
        _config.ConfigData.lookSensitivity = value;
        _config.Save();
    }
}
