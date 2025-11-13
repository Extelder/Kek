using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaAttack : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _attacklocTransform;
    public void Attack(PlayerCharacter _Location)
    {
        GameObject gameObject= Instantiate(_bullet, _attacklocTransform.transform.position, Quaternion.identity);
        BazookaBullet baz= gameObject.GetComponent<BazookaBullet>();
        baz.target = _Location;
    }
}
