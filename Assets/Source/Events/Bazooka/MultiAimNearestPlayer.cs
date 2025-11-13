using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MultiAimNearestPlayer : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _character;
    [SerializeField] private Transform _lookPoint;

    private void Start()
    {
        StartCoroutine(SearchingForPlayer());
    }

    private IEnumerator SearchingForPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            PlayerCharacter nearestCharacter = FindNearestPlayerCharacter(transform.position);
            _lookPoint.position = nearestCharacter.TargetPoint.position;
        }
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