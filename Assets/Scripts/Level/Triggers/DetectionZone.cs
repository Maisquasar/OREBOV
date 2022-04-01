using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : Trigger
{
    [HideInInspector] public bool Detected;

    private void Update()
    {
        if (Detected)
            Detected = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            Detected = true;
            Debug.Log(Detected);
        }
    }
}
