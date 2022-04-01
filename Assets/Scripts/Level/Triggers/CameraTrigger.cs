using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : Trigger
{
    [SerializeField] Camera cameraToMove;
    [Tooltip("Ctrl + Shift + F to place the cube to camera position")]
    [SerializeField] GameObject switchToCamera;
    [SerializeField] bool reverse;
    [Tooltip("In seconds at 60 fps")]
    [SerializeField] float travelTime;

    Vector3 initialPos;
    Quaternion initialRot;
    bool activate = false;

    public new void Start()
    {
        initialPos = cameraToMove.transform.position;
        initialRot = cameraToMove.transform.rotation;
        base.Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            /*
            if (activate && reverse)
            {
                cameraToMove.transform.position = initialPos;
                cameraToMove.transform.rotation = initialRot;
            }
            else 
            {
                cameraToMove.transform.position = switchToCamera.transform.position;
                cameraToMove.transform.rotation = switchToCamera.transform.rotation;
            }
            activate = !activate;
            */
            if (activate && reverse)
            {
                StartCoroutine(LerpFromTo(switchToCamera.transform.position, initialPos, travelTime));
                StartCoroutine(LerpFromTo(switchToCamera.transform.rotation, initialRot, travelTime));
            }
            else
            {
                StartCoroutine(LerpFromTo(initialPos, switchToCamera.transform.position, travelTime));
                StartCoroutine(LerpFromTo(initialRot, switchToCamera.transform.rotation, travelTime));
            }
            activate = !activate;
        }
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
}
