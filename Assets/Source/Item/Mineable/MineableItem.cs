using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineableItem : Item
{
    [SerializeField] private Transform _modelsOrigin;
    [SerializeField] private float _scaleDifference;
    [SerializeField] private InteractItem _interactItem;
    
    public override void Interact()
    {
        if (_modelsOrigin.localScale.x <= _scaleDifference || _modelsOrigin.localScale.y <= _scaleDifference || _modelsOrigin.localScale.z <= _scaleDifference)
        {
            _interactItem.DespawnObject();
            return;
        }
        _modelsOrigin.localScale -= new Vector3(_scaleDifference, _scaleDifference, _scaleDifference);
    }
}
