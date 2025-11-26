using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VolumeLerp : MonoBehaviour
{
    [SerializeField] private PostProcessVolume _volume;

    [SerializeField] private float _lerpSpeed;
    [SerializeField] private float _revertSpeed;

    private CompositeDisposable _disposable = new CompositeDisposable();

    public void StartLerp()
    {
        _disposable?.Clear();

        float current = 1;

        Observable.EveryUpdate().Subscribe(_ =>
        {
            Debug.Log("Lerping");

            if (current == 0)
            {
                _volume.weight = Mathf.Lerp(_volume.weight, current, _revertSpeed * Time.deltaTime);

                if (_volume.weight <= 0.1f)
                {
                    _volume.weight = 0;
                    _disposable.Clear();
                }

                return;
            }

            _volume.weight = Mathf.Lerp(_volume.weight, current, _lerpSpeed * Time.deltaTime);
            if (current - _volume.weight <= 0.1f)
            {
                current = 0;
            }
        }).AddTo(_disposable);
    }

    private void OnDisable()
    {
        _disposable?.Clear();
    }
}