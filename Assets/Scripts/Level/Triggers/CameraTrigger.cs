using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraTrigger : Trigger
{
    private Transform endPos;
    private Transform startPos;
    [Tooltip("Does the camera enter free movement after traveling ?")]
    [SerializeField] private bool _resetFreeMovement = true;
    [Tooltip("Can the trigger go from start to end, and from end to start if activated again ?")]
    [SerializeField] private bool reverse;
    [Tooltip("The travel time for the camera in seconds")]
    [SerializeField] private float _startTravelTime = 1.0f;
    [Tooltip("The reversed travel time for the camera in seconds (only if \"Reverse\" is active)")]
    [SerializeField] private float _endTravelTime = 1.0f;
    [Tooltip("The detection plane at which the trigger activates (only if \"Reverse\" is active)")]
    [SerializeField] private float _detectionPlane = 0f;

    [Header("Player Setting")]
    [Tooltip("Does the player need to be in an specific state for the trigger to activate ?")]
    [SerializeField] private bool _checkPlayerState;
    [Tooltip("The required state of the player (only if \"Check Player State\" is set to true)")]
    [SerializeField] private bool _isShadow;

    private Camera _cameraToMove;
    private CameraBehavior _cameraBehavior;
    private PlayerStatus _playerStatus;

    private bool _isActive = false;
    private bool _inAnim = false;
    private bool _playerSide = false;

    public override void Start()
    {
        endPos = transform.Find("SwitchTo");
        if (!endPos) Debug.LogError("Cannot find child \"SwitchTo\", did you renamed it ?");
        startPos = transform.Find("SwitchFrom");
        if (!startPos) Debug.LogError("Cannot find child \"SwitchFrom\", did you renamed it ?");

        _cameraToMove = Camera.main;
        _cameraBehavior = _cameraToMove.GetComponent<CameraBehavior>();

        base.Start();
    }

    public void OnDrawGizmos()
    {
        if (reverse)
        {
            Gizmos.DrawCube(transform.position + new Vector3(transform.localScale.x * _detectionPlane,0,0), new Vector3(0, transform.localScale.y+1, transform.localScale.z+1));
        }
    }

    // Check if the player 
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!_playerStatus) _playerStatus = other.GetComponent<PlayerStatus>();
        if (!reverse)
        {
            if (!_isActive)
            {
                if (_checkPlayerState && (_playerStatus.IsShadow != _isShadow)) return;
                _isActive = true;
                if (_inAnim) StopAllCoroutines();
                StartCoroutine(LerpMovement(_cameraToMove.transform, endPos, false));
            }
        }
        else
        {
            _playerSide = GetCurrentSide();
            if (_checkPlayerState && (_playerStatus.IsShadow != _isShadow)) return;
            if (_playerSide != _isActive)
            {
                if (_inAnim) StopAllCoroutines();
                StartCoroutine(LerpMovement(_cameraToMove.transform, _isActive ? startPos : endPos, _isActive));
                _isActive = !_isActive;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if (!reverse) _isActive = false;
    }

    IEnumerator LerpMovement(Transform initialPos, Transform destPos, bool toEnd)
    {
        _inAnim = true;
        Vector3 stPos = initialPos.position;
        Vector3 stRot = initialPos.rotation.eulerAngles;
        float totalTime = toEnd ? _endTravelTime : _startTravelTime;
        float timer = totalTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            _cameraToMove.transform.position = Vector3.Lerp(stPos, destPos.position, (float)(1-timer/totalTime));
            Quaternion rot = initialPos.rotation;
            rot.eulerAngles = new Vector3(Mathf.LerpAngle(stRot.x, destPos.rotation.eulerAngles.x, 1 - timer / totalTime),
                Mathf.LerpAngle(stRot.y, destPos.rotation.eulerAngles.y, 1 - timer / totalTime),
                Mathf.LerpAngle(stRot.z, destPos.rotation.eulerAngles.z, 1 - timer / totalTime));
            _cameraToMove.transform.rotation = rot;
            yield return Time.deltaTime;
        }
        if (_resetFreeMovement)
        {
            _cameraBehavior.DeactiveFreeMode();
        }
        _inAnim = false;
        yield return null;
    }

    private bool GetCurrentSide()
    {
        return _playerStatus.transform.position.x > (transform.position.x + _detectionPlane);
    }
}
