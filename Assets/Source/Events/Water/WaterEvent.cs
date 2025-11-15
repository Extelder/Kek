using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class WaterEvent : RandomEvent
{
    private float _defaultGravity;

    public override void StartEvent()
    {
        _defaultGravity = Physics.gravity.y;
        SetWater(0, true);
    }

    private void OnEnable()
    {
        _defaultGravity = Physics.gravity.y;
    }

    private void OnDisable()
    {
        Physics.gravity = new Vector3(0, _defaultGravity, 0);

        foreach (var ch in PlayerCharacter.Instance.Characters)
        {
            if (ch != null)
                ch.PlayerMovement.CanFly = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetWater(float gravity, bool canFly)
    {
        SetWaterObserver(gravity, canFly);
    }

    [ObserversRpc(BufferLast = true)]
    private void SetWaterObserver(float gravity, bool canFly)
    {
        StartCoroutine(ApplyWaterEffect(gravity, canFly));
    }

    private IEnumerator RestoreGravityNextFrame()
    {
        yield return null;
        SetWater(_defaultGravity, false);
    }

    private IEnumerator ApplyWaterEffect(float gravity, bool canFly)
    {
        yield return new WaitUntil(() => PlayerCharacter.Instance != null);

        Physics.gravity = new Vector3(0, gravity, 0);

        foreach (var ch in PlayerCharacter.Instance.Characters)
        {
            if (ch != null)
                ch.PlayerMovement.CanFly = canFly;
        }
    }
}