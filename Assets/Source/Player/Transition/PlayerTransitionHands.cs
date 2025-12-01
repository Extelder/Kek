using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using Observable = UniRx.Observable;

public class PlayerTransitionHands : MonoBehaviour
{
    [SerializeField] private float _zPointerOffset = 1.202f;

    [SerializeField] private WeaponSway _sway;
    [SerializeField] private RaycastSettings _raycastSettings;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private Transform _cameraOrigin;
    [SerializeField] private CameraHeadBob _cameraHeadBob;
    [SerializeField] private Ease _ease;
    [SerializeField] private float _duration;

    [SerializeField] private Animator _animator;

    [SerializeField] private string _startTriggerName;
    [SerializeField] private string _stopTriggerName;

    private CompositeDisposable _raycastCheckDisposable = new CompositeDisposable();
    private Tween _tween;

    private Vector3 _defaultPosition;
    private Vector3 _defaultFingerPosition;

    private CinemachinePOV _cinemachinePov;
    private PlayerCharacter _character;
    private VendingMachine _vendingMachine;

    public static event Action TargetDestinated;
    public static event Action BackedToDefault;

    private void OnEnable()
    {
        VendingMachineItem.Interacted += OnInteracted;
        VendingMachineReturn.Returned += OnReturned;
    }

    private void Awake()
    {
        _character = PlayerCharacter.Instance;
        _cinemachinePov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        _defaultFingerPosition = _character.FingerLookAtPoint.position;
    }

    private void OnInteracted(Transform target, VendingMachine vendingMachine)
    {
        GameCursor.Instance.Show();
        RaycastCheck();
        _vendingMachine = vendingMachine;
        _defaultPosition = _cameraOrigin.position;
        _cameraHeadBob.EnableAnimator(false);
        _character.Rigidbody.isKinematic = true;
        _sway.enabled = false;
        _tween = _cameraOrigin.DOMove(target.position, _duration).OnComplete(() =>
        {
            _character.CameraTransform.eulerAngles = target.eulerAngles;
            TargetDestinated?.Invoke();
            _character.SetCinemachienCameraValueZero();
            _virtualCamera.enabled = false;
            _character.FingerLookAtPoint.position = _defaultFingerPosition;
            _tween?.Kill();
        }).SetEase(_ease);
        _animator.SetTrigger(_startTriggerName);
    }


    private void OnReturned()
    {
        GameCursor.Instance.Hide();
        BackedToDefault?.Invoke();
        _virtualCamera.enabled = true;
        _sway.enabled = true;
        _tween = _cameraOrigin.DOMove(_defaultPosition, _duration).OnComplete(() =>
        {
            _cameraHeadBob.EnableAnimator(true);
            _character.SetCinemachineCameraDefaultValue();
            _character.Rigidbody.isKinematic = false;
            _vendingMachine.Set(true);
            PlayerCharacter.Instance.SwitchHands();
            _tween?.Kill();
        }).SetEase(_ease);
        _animator.SetTrigger(_stopTriggerName);
    }

    private void RaycastCheck()
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hitted = Physics.Raycast(ray, out hit,
                _raycastSettings.MaxDistance, _raycastSettings.LayerMask);
            if (hitted)
            {
                if (hit.collider.TryGetComponent<Monitor>(out Monitor Monitor))
                {
                    _character.FingerLookAtPoint.position =
                        new Vector3(hit.point.x, hit.point.y, transform.forward.z * _zPointerOffset);
                }
            }
        }).AddTo(_raycastCheckDisposable);
    }

    private void OnDisable()
    {
        VendingMachineItem.Interacted -= OnInteracted;
        VendingMachineReturn.Returned -= OnReturned;
        _raycastCheckDisposable.Clear();
        _tween?.Kill();
    }
}