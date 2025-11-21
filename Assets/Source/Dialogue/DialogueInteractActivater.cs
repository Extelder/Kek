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

public class DialogueInteractActivater : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialogue _dialogue;
    [SerializeField] private float _lookAtDuration;
    [SerializeField] private Transform _head;

    public void Interact()
    {
        transform.DOLookAt(PlayerCharacter.Instance.PlayerTransform.position, _lookAtDuration, AxisConstraint.Y);
        _head.DOLookAt(PlayerCharacter.Instance.Camera.transform.position, _lookAtDuration);
        PlayerCharacter.Instance.PlayerDialogue.StartDialogue(_dialogue);
    }

    public void InteractCancelled()
    {
    }
}