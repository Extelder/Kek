using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using UnityEngine;

public class BottleFlipChalange : NetworkBehaviour
{
    [SerializeField] private InteractItem _boxInteract;
    private BoxInteract interact => (BoxInteract) _boxInteract.Item;
    [SerializeField] private float uprightThreshold;
    [SerializeField] public float stopVelocity = 0.1f;
    [SerializeField] public float stopAngularVelocity = 1f;
    private bool _check;
    private Rigidbody rb;
    private bool rewardOne;
    private bool rewardTwo;

    private void OnEnable()
    {
        interact.InteractExplosion += Interact;
    }

    private void OnDisable()
    {
        interact.InteractExplosion -= Interact;
    }

    private void Interact()
    {
        StartCoroutine(BeforeUpdate());
    }

    private IEnumerator BeforeUpdate()
    {
        yield return new WaitForSeconds(0.5f);
        _check = true;
        
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!_check) return;
        if (rb.velocity.magnitude > stopVelocity) return;
        if (rb.angularVelocity.magnitude > stopAngularVelocity) return;
        float angle = Vector3.Angle(transform.up, Vector3.up);

        if (angle < uprightThreshold)
        {
            if (!rewardOne)
            {
                AddToWallet(5);
                rewardOne = true;
            }

        }
        else if (angle > 160f)
        {
            if (!rewardTwo)
            {
                AddToWallet(15);
                rewardTwo = true;
            }
        }

        _check = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddToWallet(int money)
    {
        PlayerCharacter.Instance.PlayerWallet.AddObserever(money);
    }
}