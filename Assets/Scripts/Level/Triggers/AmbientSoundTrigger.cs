using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundTrigger : Trigger
{

    [SerializeField] private AmbientSoundType _switchTo;

    private void OnTriggerEnter(Collider other)
    {
        Component t = other.GetComponent(typeof(AmbientTypeHolder));
        if (t != null)
        {
            AmbientTypeHolder entity = (AmbientTypeHolder)t;
            entity.AmbientType = _switchTo;
        }
    }
}
