using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundTrigger : Trigger
{

    [SerializeField] private AmbientSoundType _switchTo;

    private void OnTriggerEnter(Collider other)
    {
        Component t = other.GetComponent(typeof(EntityMovement));
        if (t != null)
        {
            EntityMovement entity = (EntityMovement)t;
            entity.AmbientType = _switchTo;
        }
    }
}
