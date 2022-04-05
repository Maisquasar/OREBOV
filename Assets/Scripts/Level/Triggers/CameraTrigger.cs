using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : Trigger
{
    [Tooltip("Ctrl + Shift + F to place the cube to camera position")]
    [SerializeField] GameObject switchToCamera;
    [SerializeField] bool reverse;

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
        var tmp = _initialPos;
        _initialPos = switchToCamera.transform.position;
        switchToCamera.transform.position = tmp;
        var tmp2 = _initialRot;
        _initialRot = switchToCamera.transform.rotation;
        switchToCamera.transform.rotation = tmp2;
    }
}
