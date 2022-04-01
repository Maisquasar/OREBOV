using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : Trigger
{
    [SerializeField] Light LightObject;

    private void OnTriggerEnter(Collider other)
    {
        //TODO : add enemies
        if (other.gameObject.GetComponent<Player>())
        {
            LightObject.enabled = !LightObject.enabled;
        }
    }
}
