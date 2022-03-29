using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CollectableScript : MonoBehaviour
{
    public abstract void Collect(PauseMenuScript controller);
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            Component t = other.GetComponent(typeof(PlayerController));
            if (t != null)
            {
                Collect(((PlayerController)t).GetComponentInChildren<PauseMenuScript>());
            }
        }
    }
}
