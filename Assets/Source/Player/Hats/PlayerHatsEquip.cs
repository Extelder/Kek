using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerHatsEquip : NetworkBehaviour
{
    [SerializeField] private GameObject[] _hats;
    public override void OnStartClient()
    {
        for (int i = 0; i < _hats.Length; i++)
        {
            _hats[i].SetActive(false);
        }
    }

    public void ActivateHat(int index)
    {
        for (int i = 0; i < _hats.Length; i++)
        {
            PlayerCharacter.Instance.SetObjectEnableServer(_hats[i],false);
        }
        PlayerCharacter.Instance.SetObjectEnableServer(_hats[index],true);
    }
}
