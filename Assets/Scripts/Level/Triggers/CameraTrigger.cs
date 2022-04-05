using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraTrigger : Trigger
{
    [SerializeField] CameraSettings cameraToMove;
    [Tooltip("Ctrl + Shift + F to place the cube to camera position")]
    [SerializeField] bool reverse;

    [Tooltip("In seconds at 60 fps")]
    [SerializeField] float travelTime;

    List<CameraCheckPoint> switchToCamera = new List<CameraCheckPoint>();
    CameraCheckPoint InitialPos;
    bool CoroutineEnd = true;
    bool activate = false;
    [SerializeField]
    private bool _resetFreeMouvement;


    private Camera _cameraToMove;
    private CameraBehavior _cameraBehavior;
    private Vector3 _initialPos;
    private Quaternion _initialRot;

    public override void Start()
    {
        InitCamera();
        for (int i = 0; i < transform.childCount; i++)
        {
            switchToCamera.Add(transform.GetChild(i).GetComponent<CameraCheckPoint>());
        }
        InitialPos = Instantiate<CameraCheckPoint>(switchToCamera[0]);
        InitialPos.transform.position = cameraToMove.transform.position;
        InitialPos.transform.rotation = cameraToMove.transform.rotation;
        switchToCamera.Insert(0, InitialPos);
        base.Start();
    }

    private void InitCamera()
    {
        _cameraToMove = Camera.main;
        _cameraBehavior = _cameraToMove.GetComponent<CameraBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") 
        {
            InitiateMouvement();
        }
    }

    private void InitiateMouvement()
    {
        _cameraBehavior.ActiveFreeMode();
        _initialPos = _cameraToMove.transform.position;
        _initialRot = _cameraToMove.transform.rotation;
        StartCoroutine(LerpFromTo(_initialPos, switchToCamera.transform.position, travelTime));
        StartCoroutine(LerpFromTo(_initialRot, switchToCamera.transform.rotation, travelTime));
        if (reverse)
            Swap();
    }


        if (other.gameObject.GetComponent<Player>() && CoroutineEnd && !cameraToMove.FollowPlayer)
        {
            switchToCamera[0].transform.position = cameraToMove.transform.position;
            switchToCamera[0].transform.rotation = cameraToMove.transform.rotation;
            if (!activate || reverse)
                StartCoroutine(GoTo(switchToCamera));
        }
    }



    IEnumerator GoTo(List<CameraCheckPoint> switchTo)
    {
        CoroutineEnd = false;
        activate = true;
        for (int i = 0; i < switchTo.Count - 1; i++)
        {
            StartCoroutine(LerpFromTo(switchTo[i].transform.position, switchTo[i + 1].transform.position, switchTo[i + 1].TravelTime));
            yield return StartCoroutine(LerpFromTo(switchTo[i].transform.rotation, switchTo[i + 1].transform.rotation, switchTo[i + 1].TravelTime));
        }
        if (reverse)
            Swap();
        CoroutineEnd = true;
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
        EndMouvement();
    }

    private void EndMouvement()
    {
        if(_resetFreeMouvement)
            _cameraBehavior.DeactiveFreeMode();
    }

    // Swap values between startPos and SwitchTo.
    void Swap()
    {
        switchToCamera.Reverse();
    }
}
