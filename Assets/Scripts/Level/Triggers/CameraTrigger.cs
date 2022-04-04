using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraTrigger : Trigger
{
    [SerializeField] Camera cameraToMove;
    [Tooltip("Ctrl + Shift + F to place the cube to camera position")]
    [SerializeField] bool reverse;

    List<CameraCheckPoint> switchToCamera = new List<CameraCheckPoint>();
    CameraCheckPoint InitialPos;
    bool CoroutnineEnd = true;
    bool activate = false;

    public new void Start()
    {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() && CoroutnineEnd)
        {
            switchToCamera[0].transform.position = cameraToMove.transform.position;
            switchToCamera[0].transform.rotation = cameraToMove.transform.rotation;
            if (!activate || reverse)
                StartCoroutine(GoTo(switchToCamera));
        }
    }



    IEnumerator GoTo(List<CameraCheckPoint> switchTo)
    {
        CoroutnineEnd = false;
        activate = true;
        for (int i = 0; i < switchTo.Count - 1; i++)
        {
            StartCoroutine(LerpFromTo(switchTo[i].transform.position, switchTo[i + 1].transform.position, switchTo[i + 1].TravelTime));
            yield return StartCoroutine(LerpFromTo(switchTo[i].transform.rotation, switchTo[i + 1].transform.rotation, switchTo[i + 1].TravelTime));
        }
        if (reverse)
            Swap();
        CoroutnineEnd = true;
    }


    IEnumerator LerpFromTo(Vector3 initial, Vector3 goTo, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            cameraToMove.transform.position = Vector3.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        cameraToMove.transform.position = goTo;
    }

    IEnumerator LerpFromTo(Quaternion initial, Quaternion goTo, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            cameraToMove.transform.rotation = Quaternion.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        cameraToMove.transform.rotation = goTo;
    }

    // Swap values between startPos and SwitchTo.
    void Swap()
    {
        switchToCamera.Reverse();
    }
}
