using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolEyesLookAt : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _character;

    private void Update()
    {
        PlayerCharacter nearestCharacter = FindNearestPlayerCharacter(transform.position);
        transform.LookAt(nearestCharacter.TargetPoint.position, transform.up);
    }

    private PlayerCharacter FindNearestPlayerCharacter(Vector3 fromPosition)
    {
        PlayerCharacter[] characters = PlayerCharacter.Instance.Characters.ToArray();
        PlayerCharacter nearest = null;
        float minDistSq = float.MaxValue;

        foreach (var character in characters)
        {
            if (character == null || _character == character) continue;

            float distSq = (character.transform.position - fromPosition).sqrMagnitude;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                nearest = character;
            }
        }

        return nearest;
    }
}