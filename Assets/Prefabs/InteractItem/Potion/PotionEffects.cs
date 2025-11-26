using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PotionType
{
    Blue,
    Red,
    Green,
    Pink
}

public class PotionEffects : MonoBehaviour
{
    [SerializeField] private PotionType _potionType;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Material blueMat;
    [SerializeField] private Material redMat;
    [SerializeField] private Material greenMat;
    [SerializeField] private Material pinkMat;

    public void Drink()
    {
        switch (_potionType)
        {
            case PotionType.Blue:
                DrinkBlue();
                break;

            case PotionType.Red:
                DrinkRed();
                break;

            case PotionType.Green:
                DrinkGreen();
                break;

            case PotionType.Pink:
                DrinkPink();
                break;
        }
    }

    private void DrinkBlue()
    {
        _renderer.sharedMaterial = blueMat;
    }

    private void DrinkGreen()
    {
        _renderer.sharedMaterial = greenMat;
    }

    private void DrinkPink()
    {
        _renderer.sharedMaterial = pinkMat;
    }

    private void DrinkRed()
    {
        _renderer.sharedMaterial = redMat;
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