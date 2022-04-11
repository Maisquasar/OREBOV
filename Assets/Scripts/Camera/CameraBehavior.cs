using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraBehavior : MonoBehaviour
{

    private enum CameraBehaviorState
    {
        FollowTarget,
        FreeMovement,
    }

    [SerializeField]
    private CameraBehaviorState _camState;

    [Header("Camera Window")]
    [SerializeField]
    private GameObject _mainTarget;

    [Space]
    [SerializeField]
    private bool _showWindow;

    [SerializeField]
    private Vector2 _windowSize;

    [SerializeField]
    private Vector2 _windowOffset;


    [SerializeField]
    [Range(0f, 0.1f)]
    private float _camWindownSpeed;

    [SerializeField]
    [Range(0f, 0.1f)]
    private float _camSpeed;

    private PlayerStatus _player;
    private Vector3 _windowCenter;
    private Vector3 _windowOrigin;
    private Vector2 dir;
    private bool _inScreen;


    public void Start()
    {
        Vector3 naturalOffset = transform.position - _mainTarget.transform.position;
        transform.position = new Vector3(_mainTarget.transform.position.x - _windowOffset.x ,_mainTarget.transform.position.y + naturalOffset.y, transform.position.z);
        _windowOrigin = _windowCenter + (Vector3)(_windowSize / 2f);
        _player = _mainTarget.GetComponent<PlayerStatus>();
    
    }

    private void FixedUpdate()
    {
       if(_camState == CameraBehaviorState.FollowTarget) UpdateFollowMode();
    }

    private void UpdateFollowMode()
    {

        SetWindowPosition();
        _inScreen = WindownCamContains(_mainTarget.transform.position);
        FollowPlayer();
    }

    private void SetWindowPosition()
    {
        _windowCenter = Vector3.Lerp(_windowCenter, new Vector3(transform.position.x,transform.position.y,0f)+ (Vector3)_windowOffset, _camWindownSpeed);
        _windowCenter.z = _mainTarget.transform.position.z;
        _windowOrigin = _windowCenter - (Vector3)(_windowSize / 2f);
    }

    private void FollowPlayer()
    {
        if (!WindownCamContains(_mainTarget.transform.position))
        {
            Vector3 target =  Vector3.zero;
            if (!WindownCamContainsX(_mainTarget.transform.position)) target += new Vector3(_mainTarget.transform.position.x, 0f, 0f) - new Vector3(_windowCenter.x, 0f, 0f);
            if (!WindownCamContainsY(_mainTarget.transform.position)) target += new Vector3(0f,_mainTarget.transform.position.y, 0f) - new Vector3(0f ,_windowCenter.y, 0f);

            transform.position += Vector3.Lerp(Vector3.zero, target, _camSpeed);
        }
    }


    private void OnDrawGizmos()
    {

        if (_mainTarget == null)
        {
            Debug.LogError("Missing Player");
        }
        if (_showWindow)
        {
            SetWindowPosition();
            DrawRectWindown();
        }

    }


    private void DrawRectWindown()
    {
        Gizmos.color = Color.blue;
        SetWindowPosition();
        Gizmos.DrawLine(_windowOrigin, _windowOrigin + new Vector3(_windowSize.x, 0f, 0f));
        Gizmos.DrawLine(_windowOrigin + new Vector3(_windowSize.x, 0f, 0f), _windowOrigin + (Vector3)_windowSize);
        Gizmos.DrawLine(_windowOrigin + (Vector3)_windowSize, _windowOrigin + new Vector3(0, _windowSize.y, 0f));
        Gizmos.DrawLine(_windowOrigin + new Vector3(0, _windowSize.y, 0f), _windowOrigin);
    }

    private bool WindownCamContains(Vector3 position)
    {

        if (position.x < _windowOrigin.x || position.x > _windowOrigin.x + _windowSize.x) return false;
        if (position.y < _windowOrigin.y || position.y > _windowOrigin.y + _windowSize.y) return false;


        return true;
    }

    private bool WindownCamContainsX(Vector3 position)
    {

        if (position.x < _windowOrigin.x || position.x > _windowOrigin.x + _windowSize.x) return false;


        return true;
    }

    private bool WindownCamContainsY(Vector3 position)
    {
        if (position.y < _windowOrigin.y || position.y > _windowOrigin.y + _windowSize.y) return false;


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
