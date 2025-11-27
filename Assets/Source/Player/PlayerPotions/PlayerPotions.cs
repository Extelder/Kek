using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerPotions : NetworkBehaviour
{
    [SerializeField] private MixSoundAndPlay _audio;

    [ServerRpc(RequireOwnership = false)]
    public void DrinkBlue()
    {
        ObserverStartBlueEffect();
    }

    [ObserversRpc]
    private void ObserverStartBlueEffect()
    {
        _audio.MixOnServer();
        StartCoroutine(DisableGravity());
    }

    private IEnumerator DisableGravity()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;

        float moonGravity = -1.62f;
        float time = 0f;
        rigidbody.AddForce(Vector3.up * 30f, ForceMode.Impulse);
        while (time < 15f)
        {
            rigidbody.AddForce(Vector3.up * moonGravity, ForceMode.Acceleration);
            time += Time.deltaTime;
            yield return null;
        }

        rigidbody.useGravity = true;
    }

    public void DrinkGreen()
    {
    }

    public void DrinkPink()
    {
    }

    public void DrinkRed()
    {
    }
}