using System;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class CameraBehavior : MonoBehaviour
{
    private enum CameraBehaviorState
    {
        FollowTarget,
        FreeMovement,
    }

    [SerializeField] private CameraBehaviorState _camState;

    [Header("Camera Window")]
    [SerializeField] private GameObject _mainTarget;
    [SerializeField] private bool _showWindow;
    public Vector2 WindowSize;
    public Vector2 WindowOffset;

    [Range(0f, 10f)]
    [SerializeField] private float _camWindownSpeed;
    [Range(0f, 10000f)]
    [SerializeField] private float _camSpeed;

    [Range(0, 100f)]
    [SerializeField] private float _stopCamDistance;
    [SerializeField] private LayerMask _camLayer;
    [SerializeField] private Canvas _debugCanvas;
    [SerializeField] private TextMeshProUGUI _fpsCounter;
    [SerializeField] private Texture2D tex;
    private float _maxFPS = 60.0f;
    private float _minFPS = 60.0f;
    private int _timer;

    // Checkpoint variable
    [HideInInspector] public Vector2 WindowSizeCheckpoint;
    [HideInInspector] public Vector2 WindowOffsetCheckpoint;
    [HideInInspector] public Vector3 PositionCheckpoint;
    [HideInInspector] public Quaternion RotationCheckpoint;

    private PlayerStatus _player;
    private Vector3 _windowCenter;
    private Vector3 _windowOrigin;
    private Vector2 _dir;


    public void Start()
    {
        InitCameraPosition();
        InitComponents();
    }

    #region Initialisation
    private void InitComponents()
    {
        _player = _mainTarget.GetComponent<PlayerStatus>();
    }

    private void InitCameraPosition()
    {

        Vector3 naturalOffset = transform.position - _mainTarget.transform.position;
        transform.position = new Vector3(_mainTarget.transform.position.x, _mainTarget.transform.position.y + naturalOffset.y, transform.position.z);
        _windowCenter = transform.position + (Vector3)WindowOffset;
        _windowOrigin = _windowCenter + (Vector3)(WindowSize / 2f);
    }

    #endregion

    private void Update()
    {
        RefreshFPS();
    }
    private void FixedUpdate()
    {
        if (_camState == CameraBehaviorState.FollowTarget) UpdateFollowMode();
    }

    private void UpdateFollowMode()
    {
        SetWindowPosition();
        FollowPlayer();
    }


    /// <summary>
    /// Set the window position by the camera position
    /// </summary>
    private void SetWindowPosition()
    {
        _windowCenter = Vector3.Lerp(_windowCenter, new Vector3(transform.position.x, transform.position.y, _mainTarget.transform.position.z) + (Vector3)WindowOffset, _camWindownSpeed);
        _windowCenter.z = _mainTarget.transform.position.z;
        _windowOrigin = _windowCenter - (Vector3)(WindowSize / 2f);
    }

    /// <summary>
    /// Set the window position by the camera position
    /// </summary>
    private Vector3 SetWindowPosition(Vector3 pos)
    {

        Vector3 _windowCenterL = Vector3.Lerp(_windowCenter, new Vector3(pos.x, pos.y, 0f) + (Vector3)WindowOffset, _camWindownSpeed);
        _windowCenterL.z = _mainTarget.transform.position.z;
        return _windowCenterL - (Vector3)(WindowSize / 2f);
    }

    private void FollowPlayer()
    {

        if (!WindownCamContains(_mainTarget.transform.position))
        {
            Vector3 target = Vector3.zero;

            if (!WindownCamContainsX(_mainTarget.transform.position)) target.x = _mainTarget.transform.position.x - _windowCenter.x + -Mathf.Sign(_mainTarget.transform.position.x - _windowCenter.x) * (WindowSize.x / 2f);
            if (!WindownCamContainsY(_mainTarget.transform.position)) target.y = _mainTarget.transform.position.y - _windowCenter.y + -Mathf.Sign(_mainTarget.transform.position.y - _windowCenter.y) * (WindowSize.y / 2f);

            if (!CheckWall())
                transform.position += Vector3.Lerp(Vector3.zero, target, _camSpeed);

        }


    }

    private bool CheckWall()
    {
        // Raycast from target to camera center
        float distance = transform.position.x + _stopCamDistance - _mainTarget.transform.position.x;
        float sign = Mathf.Sign(distance);
        distance = -sign * Mathf.Clamp(Mathf.Abs(distance), 0, WindowOffset.x);
        Vector3 pos = new Vector3(_mainTarget.transform.position.x, _mainTarget.transform.position.y, _mainTarget.transform.position.z);
        RaycastHit hit = new RaycastHit();
        return Physics.Raycast(pos, Vector3.right, out hit, distance, _camLayer, QueryTriggerInteraction.Ignore);
    }

    public void ToggleDebug(CallbackContext context)
    {
        if (context.started)
            _debugCanvas.enabled = !_debugCanvas.enabled;
    }

    private void RefreshFPS()
    {
        if (!_debugCanvas.enabled) return;
        float fps = 1 / Time.deltaTime;
        if (_timer != (int)(Time.time/5))
        {
            _timer = (int)(Time.time/5);
            _minFPS = fps;
            _maxFPS = fps;
        }
        else
        {
            if (fps > _maxFPS) _maxFPS = fps;
            if (fps < _minFPS) _minFPS = fps;
        }
        _fpsCounter.text = String.Format("FPS:    {0: 0.0}\nMinFPS: {1: 0.0}\nMaxFPS: {2: 0.0}\nDeltaTime: {3: 0.0}", fps, _minFPS, _maxFPS, Time.deltaTime*1000);
        refreshGraph();
    }

    private void refreshGraph()
    {
        Color32[] array = tex.GetPixels32();
        for (int i = 0; i < tex.height; i++)
        {
            for (int j = 0; j < tex.width-1; j++)
            {
                int index = i * tex.width + j;
                array[index] = array[index + 1];
            }
        }
        float timeMs = Time.deltaTime * 1000;
        Color32 color = timeMs < 17.5f ? new Color32(0, 255, 0, 255) : (timeMs < 35.0f ? new Color32(255, 255, 0, 255) : new Color32(255,0,0,255));
        int max = (timeMs > 100) ? tex.height : (int)(timeMs / 100 * tex.height);
        for (int i = 0; i < tex.height; i++)
        {
            array[(i + 1) * tex.width - 1] = (i < max) ? color : new Color32(0,0,0,0);
        }
        tex.SetPixels32(array);
        tex.Apply();
    }


    private void OnDrawGizmos()
    {

        if (_mainTarget == null)
        {
            Debug.LogError("Missing Player");
        }
        else if (_showWindow)
        {
            SetWindowPosition();
            DrawRectWindown();
        }
        refreshFPS();
    }


    public void ResetCamCheckpoint()
    {
        WindowOffset = WindowOffsetCheckpoint;
        WindowSize = WindowSizeCheckpoint;
        transform.position = PositionCheckpoint;
        transform.rotation = RotationCheckpoint;
    }

    private void DrawRectWindown()
    {
        Gizmos.color = Color.blue;
        SetWindowPosition();
        Gizmos.DrawLine(_windowOrigin, _windowOrigin + new Vector3(WindowSize.x, 0f, 0f));
        Gizmos.DrawLine(_windowOrigin + new Vector3(WindowSize.x, 0f, 0f), _windowOrigin + (Vector3)WindowSize);
        Gizmos.DrawLine(_windowOrigin + (Vector3)WindowSize, _windowOrigin + new Vector3(0, WindowSize.y, 0f));
        Gizmos.DrawLine(_windowOrigin + new Vector3(0, WindowSize.y, 0f), _windowOrigin);
    }

    private bool WindownCamContains(Vector3 position)
    {

        if (position.x < _windowOrigin.x || position.x > _windowOrigin.x + WindowSize.x) return false;
        if (position.y < _windowOrigin.y || position.y > _windowOrigin.y + WindowSize.y) return false;


        return true;
    }

    private bool WindownCamContainsX(Vector3 position)
    {
        if (position.x < _windowOrigin.x || position.x > _windowOrigin.x + WindowSize.x) return false;
        return true;
    }

    private bool WindownCamContainsY(Vector3 position)
    {
        if (position.y < _windowOrigin.y || position.y > _windowOrigin.y + WindowSize.y) return false;
        return true;
    }

    // Active the free mode camera
    public void ActiveFreeMode()
    {
        _camState = CameraBehaviorState.FreeMovement;
    }

    public void DeactiveFreeMode()
    {
        _camState = CameraBehaviorState.FollowTarget;
    }

}
