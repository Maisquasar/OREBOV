using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Trigger
{
    bool set = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() && !set)
        {
            set = true;
            other.GetComponent<Player>().CheckpointPos = transform.position + Vector3.up;
        }
    }
}
