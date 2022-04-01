using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public virtual void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Triggers");
    }
}
