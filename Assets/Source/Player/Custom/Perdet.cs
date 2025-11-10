using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Perdet : NetworkBehaviour
{
    [SerializeField] private float _perditTime;
    [SerializeField] private ParticleSystem _gasFx;  
    [SerializeField] private AudioSource _perditSource;
    [SerializeField] private AudioClip[] _perditClips;
    private bool _perditNow;
    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(_perditNow == false) PerdetServer(); 
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PerdetServer()
    {
        PerdetObserver();
    }
    
    [ObserversRpc]
    private void PerdetObserver()
    {
        _perditNow = true;
        _perditSource.clip = _perditClips[Random.Range(0, _perditClips.Length)];
        _perditSource.Play();
        _gasFx.Play();
        StartCoroutine(perditTime());
    }

    private IEnumerator perditTime()
    {
        yield return new WaitForSeconds(_perditTime);
        _perditSource.Stop();
        _gasFx.Stop();
        _perditNow = false;
    }
}