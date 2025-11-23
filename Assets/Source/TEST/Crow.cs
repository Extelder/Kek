using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KariteyDefender3000;

public class Crow : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _distanceThreshHold;
    [SerializeField] private Transform _target;

    private void Start()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            SafeMove.Forward(transform, _speed);
            if (SafeUtils.DistanceThreshHoldAchieved(transform, _target, _distanceThreshHold))
            {
                SafeDebug.Info("Stopped");
                StopAllCoroutines();
            }
        }
    }
}
