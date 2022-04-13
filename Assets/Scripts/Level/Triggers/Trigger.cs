using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] bool ShowOnLoad;
    public virtual void Start()
    {
        if (!ShowOnLoad)
            GetComponent<MeshRenderer>().enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Triggers");
    }
}
