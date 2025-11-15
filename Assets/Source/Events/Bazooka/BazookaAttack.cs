using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class BazookaAttack : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _attacklocTransform;
    public void Attack(PlayerCharacter _Location)
    {
        GameObject bullet = Instantiate(_bullet, _attacklocTransform.transform.position, Quaternion.identity);
        bullet.GetComponent<NetworkObject>().Spawn(bullet);
        BazookaBullet baz = bullet.GetComponent<BazookaBullet>();    
        baz.target = _Location;
    }
}
