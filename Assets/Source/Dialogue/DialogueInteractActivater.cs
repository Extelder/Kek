using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class Dialogue
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Color Color { get; private set; }
    [field: SerializeField] public Replica[] Replicas { get; private set; }
}

public abstract class DialogueEvent
{
    public abstract void Invoke();
}

public class DialogueInteractActivater : MonoBehaviour, IInteractable
{
    [SerializeField] private string _saveKey;

    [Tooltip("Last dialogue will repeat if they left")] [SerializeField]
    private Dialogue[] _dialogue;

    [SerializeField] private Dialogue _repeatDialouge;

    [SerializeField] private float _lookAtDuration;
    [SerializeField] private Transform _head;

    private int _currentDialogueID;

    private bool _dialoguingThisTime;

    private void Start()
    {
        _currentDialogueID = PlayerPrefs.GetInt(_saveKey + "CurrentDialogue", 0);
        if (_currentDialogueID > _dialogue.Length - 1)
        {
            _currentDialogueID = _dialogue.Length - 1;
            PlayerPrefs.SetInt(_saveKey + "CurrentDialogue", _currentDialogueID);
        }
    }

    public void Interact()
    {
        if (_dialoguingThisTime)
        {
            transform.DOLookAt(PlayerCharacter.Instance.PlayerTransform.position, _lookAtDuration, AxisConstraint.Y);
            _head.DOLookAt(PlayerCharacter.Instance.Camera.transform.position, _lookAtDuration);
            PlayerCharacter.Instance.PlayerDialogue.StartDialogue(_repeatDialouge);
            return;
        }

        _dialoguingThisTime = true;
        transform.DOLookAt(PlayerCharacter.Instance.PlayerTransform.position, _lookAtDuration, AxisConstraint.Y);
        _head.DOLookAt(PlayerCharacter.Instance.Camera.transform.position, _lookAtDuration);
        PlayerCharacter.Instance.PlayerDialogue.StartDialogue(_dialogue[_currentDialogueID]);
        PlayerPrefs.SetInt(_saveKey + "CurrentDialogue", _currentDialogueID + 1);
    }

    public void InteractCancelled()
    {
    }
}