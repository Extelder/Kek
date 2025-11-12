using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaBullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    public PlayerCharacter target;

    void Update()
    {
        if (target == null)
        {
            return;
        }
        Vector3 dir = (target.transform.position - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 100f * 100f * Time.deltaTime);
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        var player = collision.collider.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            Debug.Log("ТЫ СДОХАВЛОАОВАЫРПАОВЫАРАВЫОРПАВЫОАРВЫОАВЫОАВЫОАВЫОАВЫАРВЫОА");
        }

        Destroy(gameObject);
    }
}
