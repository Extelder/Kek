using System.Collections;
using System.Collections.Generic;
using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;

public class BazookaAttack : NetworkBehaviour
{
    [SerializeField] private NetworkObject _bullet;
    [SerializeField] private GameObject _attacklocTransform;
    public void Attack(PlayerCharacter _Location)
    {
        NetworkObject bullet = Instantiate(_bullet, _attacklocTransform.transform.position, Quaternion.identity);
        ServerManager.Spawn(bullet);
        BazookaBullet baz = bullet.GetComponent<BazookaBullet>();    
        baz.target = _Location;
    }
}
