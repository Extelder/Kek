using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;

public class TractorEvent : RandomEvent
{
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [SerializeField] private AudioSource _audio;
    [SerializeField] private Transform[] wheels; // все колёса, которые крутятся
    
    [Header("Refs")]

    [Tooltip("Родители рулевых (ось Y поворота)")]
    [SerializeField] private Transform[] steeringPivots;

    [Tooltip("Дочерние рулевых (ось вращения колеса)")]
    [SerializeField] private Transform[] steeringSpins; // длина = длине steeringPivots

    [Header("Params")]
    [SerializeField] private float wheelRadius = 0.35f;
    [SerializeField] private float smoothLerp = 10f;
    [SerializeField] private Vector3 wheelRotateAxis = Vector3.right; // ось спина в ЛОКАЛЕ колеса
    [SerializeField] private float directionSign = 1f; // 1 или -1
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private bool useDesiredVelocityForSteer = true;
    private float _smoothedForwardSpeed;
    private Quaternion[] _pivotInit;
    private Quaternion[] _spinInit;
    private float[] _steerSpinAngle; // аккумулируем спин для рулевых
    private PlayerCharacter _target;

    public override void StartEvent()
    {

        StartEventServer();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartEventServer()
    {
        SetDestinationObserver();
    }

    [ObserversRpc]
    private void SetDestinationObserver()
    {
        _target = FindNearestPlayerCharacter(transform.position);
        Agent.SetDestination(_target.transform.position);
        _audio.Play();
    }

    private PlayerCharacter FindNearestPlayerCharacter(Vector3 fromPosition)
    {
        PlayerCharacter[] characters = PlayerCharacter.Instance.Characters.ToArray();
        PlayerCharacter nearest = null;
        float minDistSq = float.MaxValue;

        foreach (var character in characters)
        {
            if (character == null) continue;

            float distSq = (character.transform.position - fromPosition).sqrMagnitude;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                nearest = character;
            }
        }

        return nearest;
    }

    void Awake()
    {
        if (Agent == null) Agent = GetComponent<NavMeshAgent>();

        if (steeringPivots != null && steeringPivots.Length > 0)
        {
            int n = steeringPivots.Length;
            _pivotInit = new Quaternion[n];
            _spinInit  = new Quaternion[n];
            _steerSpinAngle = new float[n];

            for (int i = 0; i < n; i++)
            {
                if (steeringPivots[i] != null) _pivotInit[i] = steeringPivots[i].localRotation;
                if (steeringSpins != null && i < steeringSpins.Length && steeringSpins[i] != null)
                    _spinInit[i] = steeringSpins[i].localRotation;
            }
        }
    }

    void Update()
    {
        if (Agent == null) return;

        // Движение агента
        if (_target != null) Agent.SetDestination(_target.transform.position); // оставь свою логику таргета

        // --- скорость вдоль forward ---
        Vector3 vel = Agent.velocity;
        float forwardSpeed = Vector3.Dot(vel, transform.forward);

        _smoothedForwardSpeed = Mathf.Lerp(
            _smoothedForwardSpeed, forwardSpeed,
            1f - Mathf.Exp(-smoothLerp * Time.deltaTime)
        );

        float r = Mathf.Max(wheelRadius, 0.0001f);
        float degThisFrame = directionSign * (_smoothedForwardSpeed / r) * Mathf.Rad2Deg * Time.deltaTime;

        // --- спин НЕ рулевых ---
        if (wheels != null)
        {
            for (int i = 0; i < wheels.Length; i++)
                if (wheels[i] != null)
                    wheels[i].Rotate(wheelRotateAxis, degThisFrame, Space.Self);
        }

        // --- руление ---
        float steerAngle = 0f;
        Vector3 steerVec = useDesiredVelocityForSteer && Agent.desiredVelocity.sqrMagnitude > 0.01f
            ? Agent.desiredVelocity
            : vel;

        if (steerVec.sqrMagnitude > 0.0001f)
        {
            Vector3 localDir = transform.InverseTransformDirection(steerVec.normalized);
            steerAngle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
            steerAngle = Mathf.Clamp(steerAngle, -maxSteerAngle, maxSteerAngle);
        }

        // --- рулевые: поворот + спин раздельно ---
        if (steeringPivots != null && steeringSpins != null)
        {
            int n = Mathf.Min(steeringPivots.Length, steeringSpins.Length);
            for (int i = 0; i < n; i++)
            {
                // поворот по Y на родителе
                if (steeringPivots[i] != null)
                    steeringPivots[i].localRotation = _pivotInit[i] * Quaternion.Euler(0f, steerAngle, 0f);

                // спин на дочернем
                if (steeringSpins[i] != null)
                {
                    _steerSpinAngle[i] += degThisFrame;
                    Quaternion spinQ = Quaternion.AngleAxis(_steerSpinAngle[i], wheelRotateAxis);
                    steeringSpins[i].localRotation = _spinInit[i] * spinQ;
                }
            }
        }
    }
}