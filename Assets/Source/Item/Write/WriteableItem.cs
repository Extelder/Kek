using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WriteableItem : Item
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private WriteableSign _writeableSign;

    public override void Interact()
    {
        _inputField.ActivateInputField();
        Debug.Log("OnInteracted");
        _writeableSign.OnInteracted(_inputField);
    }
}
