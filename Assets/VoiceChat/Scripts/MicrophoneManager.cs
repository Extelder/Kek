using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static TMPro.TMP_Dropdown;

public class MicrophoneManager : MonoBehaviour
{
    public static MicrophoneManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("[ERROR] Microphone Manager instance not found!");
            }

            return instance;
        }
    }

    private static MicrophoneManager instance;

    public TMP_Dropdown Dropdown;
    [SerializeField] private List<string> AvailableDevices;

    private int _currentMicrophone;
    void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (var device in Microphone.devices)
        {
            AvailableDevices.Add(device);
        }

        List<OptionData> options = new List<OptionData>();
        AvailableDevices.ForEach(x => options.Add(new OptionData(x)));
        Dropdown.AddOptions(options);
        Dropdown.onValueChanged.AddListener((value =>
        {
            _currentMicrophone = value;
        }));
    }

    public string GetCurrentDeviceName()
    {
        return Microphone.devices[_currentMicrophone];
    }

    private void OnDisable()
    {
        Dropdown.onValueChanged.RemoveAllListeners();
    }
}