using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class InvertEvent : RandomEvent
{
    public override void StartEvent()
    {
        SetInvert(true);
    }

    private void OnDisable()
    {
        foreach (var ch in PlayerCharacter.Instance.Characters)
        {
            if (ch != null)
                ch.PlayerMovement.Invert = 1;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetInvert(bool invert)
    {
        SetInvertObserver(invert);
    }

    [ObserversRpc(BufferLast = true)]
    private void SetInvertObserver(bool invert)
    {
        StartCoroutine(ApplyInvertEffect(invert));
    }

    private IEnumerator ApplyInvertEffect(bool invert)
    {
        yield return new WaitUntil(() => PlayerCharacter.Instance != null);

        float invertValue = 1;

        if (invert)
            invertValue = -1f;
        foreach (var ch in PlayerCharacter.Instance.Characters)
        {
            if (ch != null)
                ch.PlayerMovement.Invert = invertValue;
        }
    }
}