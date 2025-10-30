using System.Collections;
using FishNet.Object;
using UnityEngine;

[ExecuteAlways]
public class UnitPlayerDetector : NetworkBehaviour
{
    [Space(20)] [SerializeField] private int _horizontalRays = 25;
    [SerializeField] private int _verticalRays = 15;
    [SerializeField] private float _horizontalFov = 100f;
    [SerializeField] private float _verticalFov = 60f;
    [SerializeField] private float _viewDistance = 20f;
    [SerializeField] private LayerMask _visionMask;
    [SerializeField] private Transform _eyesPoint;

    [Space(20)] [SerializeField] private float _inspectTime = 0.2f;

    [SerializeField] private float _chaseTime = 0.3f;
    [SerializeField] private EnemyStateMachine _stateMachine;

    private Transform _player;
    private bool _isSeeingPlayer;
    private float _seeTimer;
    private Vector3[,] _rayGrid;
    private bool _gridDirty = true;

    private void OnValidate()
    {
        _horizontalRays = Mathf.Max(2, _horizontalRays);
        _verticalRays = Mathf.Max(2, _verticalRays);
        _horizontalFov = Mathf.Clamp(_horizontalFov, 1f, 360f);
        _verticalFov = Mathf.Clamp(_verticalFov, 1f, 180f);
        _viewDistance = Mathf.Max(0.1f, _viewDistance);
        _inspectTime = Mathf.Max(0f, _inspectTime);
        _chaseTime = Mathf.Max(_inspectTime, _chaseTime);

        _gridDirty = true;
        if (!Application.isPlaying)
            GenerateRayGrid();
    }

    public override void OnStartClient()
    {
        if (!base.IsServer)
            return;

        var playerCharacter = PlayerCharacter.Instance;
        if (playerCharacter != null)
            _player = playerCharacter.PlayerTransform;

        if (_gridDirty || _rayGrid == null)
            GenerateRayGrid();

        StartCoroutine(VisionLoop());
    }

    private void GenerateRayGrid()
    {
        if (_horizontalRays < 2) _horizontalRays = 2;
        if (_verticalRays < 2) _verticalRays = 2;

        _rayGrid = new Vector3[_horizontalRays, _verticalRays];

        float hx = Mathf.Max(1, _horizontalRays - 1);
        float vy = Mathf.Max(1, _verticalRays - 1);

        for (int y = 0; y < _verticalRays; y++)
        {
            for (int x = 0; x < _horizontalRays; x++)
            {
                float tX = x / hx;
                float tY = y / vy;
                float yaw = Mathf.Lerp(-_horizontalFov * 0.5f, _horizontalFov * 0.5f, tX);
                float pitch = Mathf.Lerp(-_verticalFov * 0.5f, _verticalFov * 0.5f, tY);
                _rayGrid[x, y] = Quaternion.Euler(-pitch, yaw, 0f) * Vector3.forward;
            }
        }

        _gridDirty = false;
    }

    private IEnumerator VisionLoop()
    {
        WaitForSeconds delay = new WaitForSeconds(0.05f);
        while (true)
        {
            if (_rayGrid == null || _gridDirty) GenerateRayGrid();

            bool detected = false;
            if (_player == null)
            {
                var pc = FindObjectOfType<PlayerCharacter>();
                if (pc != null) _player = pc.PlayerTransform;
            }

            if (_player != null)
            {
                Vector3 origin = _eyesPoint ? _eyesPoint.position : transform.position + Vector3.up * 1.6f;

                for (int y = 0; y < _verticalRays; y++)
                {
                    for (int x = 0; x < _horizontalRays; x++)
                    {
                        Vector3 dir = transform.rotation * _rayGrid[x, y];
                        if (Physics.Raycast(origin, dir, out RaycastHit hit, _viewDistance, _visionMask))
                        {
                            if (hit.collider.TryGetComponent<PlayerCharacter>(out PlayerCharacter PlayerCharacter))
                            {
                                _character = PlayerCharacter;
                                detected = true;
                                Debug.DrawRay(origin, dir * hit.distance, Color.red, 0.05f);
                            }
                            else
                            {
                                Debug.DrawRay(origin, dir * hit.distance, Color.gray, 0.05f);
                            }
                        }
                        else
                        {
                            Debug.DrawRay(origin, dir * _viewDistance, Color.green, 0.05f);
                        }
                    }
                }
            }

            UpdateAI(detected);
            yield return delay;
        }
    }

    private PlayerCharacter _character;

    private void UpdateAI(bool playerVisible)
    {
        if (playerVisible)
        {
            _seeTimer += Time.deltaTime;

            if (!_isSeeingPlayer && _seeTimer >= _inspectTime)
            {
                _isSeeingPlayer = true;
                //  _stateMachine?.Inspect(PlayerCharacter.Instance.PlayerTransform.position);
            }

            if (_seeTimer >= _chaseTime)
            {
                _stateMachine?.Chase(_character.PlayerTransform);
            }
        }
        else
        {
            _isSeeingPlayer = false;
            _seeTimer = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_rayGrid == null || _gridDirty)
            GenerateRayGrid();

        if (_rayGrid == null) return;

        Vector3 origin = _eyesPoint ? _eyesPoint.position : transform.position + Vector3.up * 1.6f;
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.7f);

        float gizmoScale = 0.4f;

        for (int y = 0; y < _verticalRays; y++)
        {
            for (int x = 0; x < _horizontalRays; x++)
            {
                Vector3 dir = transform.rotation * _rayGrid[x, y];
                Gizmos.DrawRay(origin, dir * _viewDistance * gizmoScale);
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, 0.06f);
    }

    public void MarkGridDirty() => _gridDirty = true;
}