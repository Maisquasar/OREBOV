using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Trigger
{
    private bool _set = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() && !_set)
        {
            _set = true;
            other.GetComponent<Player>().CheckpointPos = transform.position + Vector3.up;
        }
    }
}
