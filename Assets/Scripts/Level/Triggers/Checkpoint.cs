using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Trigger
{
    private bool _set = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerStatus>() && !_set)
        {
            _set = true;
            other.GetComponent<PlayerStatus>().CheckpointPos = transform.position + Vector3.up;
        }
    }
}
