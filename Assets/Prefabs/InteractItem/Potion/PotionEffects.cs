using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

enum PotionType
{
    Blue,
    Red,
    Green,
    Pink
}

public class PotionEffects : NetworkBehaviour
{
    [SerializeField] private PotionType _potionType;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material redMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material pinkMat;
    [SerializeField] private GameObject _audioGameObject;

    public void Drink()
    {
        switch (_potionType)
        {
            case PotionType.Blue:
                PlayerCharacter.Instance.PlayerPotions.DrinkBlue();
                break;

            case PotionType.Red:
                PlayerCharacter.Instance.PlayerPotions.DrinkRed();
                break;

            case PotionType.Green:
                PlayerCharacter.Instance.PlayerPotions.DrinkGreen();
                break;

            case PotionType.Pink:
                PlayerCharacter.Instance.PlayerPotions.DrinkPink();
                break;
        }
        PlayerCharacter.Instance.ServerSpawnObject(_audioGameObject, transform.position, transform.rotation);
        PlayerCharacter.Instance.DespawnObject(this);
    }

    private void OnValidate()
    {
        switch (_potionType)
        {
            case PotionType.Blue:
                _renderer.sharedMaterial = blueMat;
                break;

            case PotionType.Red:
                _renderer.sharedMaterial = redMat;
                break;

            case PotionType.Green:
                _renderer.sharedMaterial = greenMat;
                break;

            case PotionType.Pink:
                _renderer.sharedMaterial = pinkMat;
                break;
        }
    }
}