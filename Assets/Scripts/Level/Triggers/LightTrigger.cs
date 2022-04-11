using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : Trigger
{
    [SerializeField] private Light _lightObject;

    private void OnTriggerEnter(Collider other)
    {
        //TODO : add enemies
        if (other.gameObject.GetComponent<PlayerStatus>())
        {
            _lightObject.enabled = !_lightObject.enabled;
        }
    }
}
