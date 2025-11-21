using System;
using System.Collections;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


[Serializable]
public class Replica
{
    [field: SerializeField] public string Text { get; private set; }
    [field: SerializeField] public float delay { get; private set; }
}

public class PlayerDialogue : MonoBehaviour
{
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _nameText;

    [SerializeField] private PlayerCharacter _character;

    private int _currentIndex = 0;

    public bool Dialoguing { get; private set; }

    private Replica[] _currentReplics;
    private void TrySkipDialogue(InputAction.CallbackContext obj)
    {
        if (_currentReplics == null)
            return;
        StopAllCoroutines();
        StartCoroutine(ContinueDialoguing(_currentReplics, _currentIndex + 1));
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (Dialoguing)
            return;
        PlayerCharacter.Instance.Rigidbody.isKinematic = true;
        Dialoguing = true;
        _nameText.text = dialogue.Name;
        _nameText.color = dialogue.Color;
        _dialoguePanel.SetActive(true);
        _character.Binds.Character.Interact.started += TrySkipDialogue;

        _currentReplics = dialogue.Replicas;
        StopAllCoroutines();
        StartCoroutine(ContinueDialoguing(dialogue.Replicas, 0));
    }

    private IEnumerator ContinueDialoguing(Replica[] replicas, int fromStart)
    {
        _currentIndex = fromStart;

        if (_currentIndex > replicas.Length)
        {
            _currentReplics = null;
            StartCoroutine(RecoverDialoguing());
            _dialoguePanel.SetActive(false);
            yield break;
        }

        for (int i = _currentIndex; i < replicas.Length; i++)
        {
            _currentIndex = i;
            _dialogueText.text = replicas[i].Text;
            yield return new WaitForSeconds(replicas[i].delay);
        }

        _currentReplics = null;
        StartCoroutine(RecoverDialoguing());
        _dialoguePanel.SetActive(false);
    }

    private IEnumerator RecoverDialoguing()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerCharacter.Instance.Rigidbody.isKinematic = false;
        _character.Binds.Character.Interact.started -= TrySkipDialogue;
        Dialoguing = false;
    }

    private void OnDisable()
    {
        PlayerCharacter.Instance.Rigidbody.isKinematic = false;
        _character.Binds.Character.Interact.started -= TrySkipDialogue;
    }
}