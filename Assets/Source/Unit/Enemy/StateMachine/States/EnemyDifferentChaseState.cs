using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDifferentChaseState : EnemyChaseState
{
    [SerializeField] private EnemyDoubleChaseAnimator _animator;
    [SerializeField] private RaycastSettings _raycastSettings;
    [SerializeField] private float _distanceToChangeChaseStates;
    private bool _useOverrideBase = true;
    
    public override void OnStartedChasing()
    {
        CheckOnDistance();
    }

    private void CheckOnDistance()
    {
        bool hitted = Physics.Raycast(_raycastSettings.Origin.position, _raycastSettings.Origin.forward, out RaycastHit hit,
            _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
        Debug.Log("Raycasting" + _useOverrideBase);
        if (hitted)
        {
            Debug.Log("Hitted" + _useOverrideBase);
            if (hit.collider.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
            {
                if (hit.distance >= _distanceToChangeChaseStates)
                {
                    Debug.Log("ChangeState" + _useOverrideBase);
                    _useOverrideBase = false;
                    return;
                }
                Debug.Log("ChangeState" + _useOverrideBase);
                _useOverrideBase = true;
                return;
            }
        }

        CallAnimations();
    }

    public override void CallAnimations()
    {
        if (_useOverrideBase)
        {
            base.CallAnimations();
            return;
        }
        _animator.SecondRun();
    }
}