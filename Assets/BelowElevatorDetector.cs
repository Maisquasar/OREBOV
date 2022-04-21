using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BelowElevatorDetector : MonoBehaviour
{
    [HideInInspector] public bool Detect = false;



    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<InteractiveBox>() || other.GetComponent<PlayerInteraction>())
        {
            Detect = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<InteractiveBox>() || other.GetComponent<PlayerInteraction>())
        {
            Detect = false;
        }
    }
}
