using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraTrigger : Trigger
{

    [Tooltip("Ctrl + Shift + F to place the cube to camera position")]
    [SerializeField] private bool reverse;

    private List<CameraCheckPoint> switchToCamera = new List<CameraCheckPoint>();
    [SerializeField] private bool _resetFreeMouvement = true;

    [Header("Player Setting")]
    [SerializeField] private bool _checkPlayerState;
    [SerializeField] private bool _isShadow;

    private Camera _cameraToMove;
    private CameraBehavior _cameraBehavior;
    private CameraCheckPoint _initialPos;
    private PlayerStatus _playerStatus;

    private bool _isActivate = false;
    private bool _resetTrigger = true;
    private bool _goToEnd = true;


    #region Inititate Script

    public override void Start()
    {
        InitCamera();
        InitCheckpointLight();
        base.Start();
    }


    private void InitCamera()
    {
        _cameraToMove = Camera.main;
        _cameraBehavior = _cameraToMove.GetComponent<CameraBehavior>();
    }
    private void InitCheckpointLight()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            switchToCamera.Add(transform.GetChild(i).GetComponent<CameraCheckPoint>());
        }
        _initialPos = Instantiate<CameraCheckPoint>(switchToCamera[0]);
        _initialPos.transform.position = _cameraToMove.transform.position;
        _initialPos.transform.rotation = _cameraToMove.transform.rotation;
        switchToCamera.Insert(0, _initialPos);

    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        _resetTrigger = true;
        DetectPlayer(other);
    }


    private void OnTriggerStay(Collider other)
    {
        if (_resetTrigger) DetectPlayer(other);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            _resetTrigger = true;

    }


    // Check if the player 
    private void DetectPlayer(Collider other)
    {
        if (other.gameObject.tag == "Player" && _goToEnd)
        {

            GetPlayerStatus(other);
            if (_checkPlayerState && _playerStatus.IsShadow == _isShadow)
            {
                ActiveCameraMove();
                return;
            }
            ActiveCameraMove();
        }
    }

    private void GetPlayerStatus(Collider other)
    {
        if (_playerStatus == null)
            _playerStatus = other.gameObject.GetComponent<PlayerStatus>();
    }

    private void ActiveCameraMove()
    {
        switchToCamera[0].transform.position = _cameraToMove.transform.position;
        switchToCamera[0].transform.rotation = _cameraToMove.transform.rotation;
        _resetTrigger = false;
        if (!_isActivate || reverse)
            StartCoroutine(GoTo(switchToCamera));
    }

   


    IEnumerator GoTo(List<CameraCheckPoint> switchTo)
    {
        _goToEnd = false;
        _isActivate = true;
        _cameraBehavior.ActiveFreeMode();
        for (int i = 0; i < switchTo.Count - 1; i++)
        {
            StartCoroutine(LerpFromTo(switchTo[i].transform.position, switchTo[i + 1].transform.position, switchTo[i + 1].TravelTime));
            yield return StartCoroutine(LerpFromTo(switchTo[i].transform.rotation, switchTo[i + 1].transform.rotation, switchTo[i + 1].TravelTime));
            if (reverse)
                Swap();
            _goToEnd = true;
            EndMouvement();
        }
    }

    IEnumerator LerpFromTo(Vector3 initial, Vector3 goTo, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            _cameraToMove.transform.position = Vector3.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        _cameraToMove.transform.position = goTo;
    }

    IEnumerator LerpFromTo(Quaternion initial, Quaternion goTo, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            _cameraToMove.transform.rotation = Quaternion.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        _cameraToMove.transform.rotation = goTo;
    }

    private void EndMouvement()
    {
        if (_resetFreeMouvement) _cameraBehavior.DeactiveFreeMode();
        _isActivate = false;
    }

    // Swap values between startPos and SwitchTo.
    void Swap()
    {
        switchToCamera.Reverse();
    }
}
