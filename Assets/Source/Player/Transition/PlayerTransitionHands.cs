using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using Observable = UniRx.Observable;

public class PlayerTransitionHands : MonoBehaviour
{
    [SerializeField] private LayerMask _fingerLookAtRaycastLayerMask;
    [SerializeField] private float _fingerLookAtRaycastLenght;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private Transform _cameraOrigin;
    [SerializeField] private float _speed;

    [SerializeField] private Animator _animator;
    [SerializeField] private string _triggerName;

    private CompositeDisposable _disposable = new CompositeDisposable();

    private Vector3 _defaultPosition;

    private CinemachinePOV _cinemachinePov;

    private PlayerCharacter _character;

    public static event Action TargetDestinated;

    private void OnEnable()
    {
        VendingMachineItem.Interacted += OnInteracted;
    }

    private void Awake()
    {
        _character = PlayerCharacter.Instance;
        _cinemachinePov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    private void OnInteracted(Transform target)
    {
        _cinemachinePov.m_HorizontalAxis.Value = 0;
        _cinemachinePov.m_VerticalAxis.Value = 0;
        _defaultPosition = _cameraOrigin.position;
        _character.Rigidbody.isKinematic = true;

        Observable.EveryUpdate().Subscribe(_ =>
        {
            _cameraOrigin.position =
                Vector3.MoveTowards(_cameraOrigin.position, target.position, _speed * Time.deltaTime);
            if (Vector3.Distance(_cameraOrigin.position, target.position) <= 0.1f)
            {
                _cinemachinePov.m_HorizontalAxis.Value = 0;
                _cinemachinePov.m_VerticalAxis.Value = 0;
                TargetDestinated?.Invoke();
                _virtualCamera.enabled = false;
                _disposable.Clear();
            }
        }).AddTo(_disposable);
        _animator.SetTrigger(_triggerName);
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _fingerLookAtRaycastLenght, _fingerLookAtRaycastLayerMask))
        {
            if (hit.collider.TryGetComponent<Monitor>(out Monitor Monitor))
            {
                _character.FingerLookAtPoint.position =
                    new Vector3(hit.point.x, hit.point.y, _character.FingerLookAtPoint.position.z);
            }
        }
    }

    private void OnDisable()
    {
        VendingMachineItem.Interacted -= OnInteracted;
        _disposable.Clear();
    }
}