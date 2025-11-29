using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerPotions : NetworkBehaviour
{
    [SerializeField] private float _pinkCooldown;
    [SerializeField] private MixSoundAndPlay _audio;
    [SerializeField] private Transform _modelRoot;

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
        rigidbody.AddForce(Vector3.up * 10f, ForceMode.Impulse);
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
        for (int i = 0; i < PlayerCharacter.Instance.Characters.Count; i++)
        {
            if (PlayerCharacter.Instance.Characters[i] != PlayerCharacter.Instance)
            {
                Vector3 point = transform.position + new Vector3(i * 1.2f, 0, 0);
                TeleportServer(point, PlayerCharacter.Instance.Characters[i]);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void TeleportServer(Vector3 point, PlayerCharacter playerCharacter)
    {
        TeleportToPlayer(point, playerCharacter);
    }

    [ObserversRpc]
    private void TeleportToPlayer(Vector3 point, PlayerCharacter playerCharacter)
    {
        transform.position = point;
        playerCharacter.transform.position = point;
    }

    public void DrinkPink()
    {
        if (!base.IsOwner)
            return;
        StopAllCoroutines();
        for (int i = 0; i < PlayerCharacter.Instance.Characters.Count; i++)
        {
            PlayerCharacter.Instance.Characters[i]._outline.enabled = true;
        }

        StartCoroutine(WaitingForPinkCooldown());
    }

    private IEnumerator WaitingForPinkCooldown()
    {
        yield return new WaitForSeconds(_pinkCooldown);
        for (int i = 0; i < PlayerCharacter.Instance.Characters.Count; i++)
        {
            PlayerCharacter.Instance.Characters[i]._outline.enabled = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DrinkRed()
    {
        DrinkRedObserver();
    }

    [ObserversRpc]
    private void DrinkRedObserver()
    {
        _modelRoot.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        StartCoroutine(WaitBaby());
    }

    private IEnumerator WaitBaby()
    {
        yield return new WaitForSeconds(76);
        _modelRoot.localScale = new Vector3(1f, 1f, 1f);
    }
}