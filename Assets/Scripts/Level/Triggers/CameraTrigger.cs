using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraTrigger : Trigger
{
 
    [Tooltip("Ctrl + Shift + F to place the cube to camera position")]
    [SerializeField] bool reverse;

    List<CameraCheckPoint> switchToCamera = new List<CameraCheckPoint>();
    CameraCheckPoint InitialPos;
    bool CoroutineEnd = true;
    bool activate = false;
    [Tooltip("In seconds at 60 fps")]
    [SerializeField] float travelTime;

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
        InitialPos.transform.position = _cameraToMove.transform.position;
        InitialPos.transform.rotation = _cameraToMove.transform.rotation;
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
        if (other.gameObject.GetComponent<Player>() && CoroutineEnd)
        {
            switchToCamera[0].transform.position = _cameraToMove.transform.position;
            switchToCamera[0].transform.rotation = _cameraToMove.transform.rotation;
            if (!activate || reverse)
                StartCoroutine(GoTo(switchToCamera));
        }
    }



    IEnumerator GoTo(List<CameraCheckPoint> switchTo)
    {
        CoroutineEnd = false;
        activate = true;
        _cameraBehavior.ActiveFreeMode();
        for (int i = 0; i < switchTo.Count - 1; i++)
        {
            StartCoroutine(LerpFromTo(switchTo[i].transform.position, switchTo[i + 1].transform.position, switchTo[i + 1].TravelTime));
            yield return StartCoroutine(LerpFromTo(switchTo[i].transform.rotation, switchTo[i + 1].transform.rotation, switchTo[i + 1].TravelTime));
            if (reverse)
                Swap();
            CoroutineEnd = true;
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
        if (_resetFreeMouvement)
            _cameraBehavior.DeactiveFreeMode();
    }

    // Swap values between startPos and SwitchTo.
    void Swap()
    {

        switchToCamera.Reverse();

    }
}
