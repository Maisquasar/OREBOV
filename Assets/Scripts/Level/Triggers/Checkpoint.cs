using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Trigger
{
    private bool _set = false;
    private CameraBehavior _cameraBehavior;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerStatus>() && !_set)
        {
            _set = true;
            other.GetComponent<PlayerStatus>().CheckpointPos = transform.position + Vector3.up;
            _cameraBehavior = Camera.main.GetComponent<CameraBehavior>();
            _cameraBehavior.WindowOffsetCheckpoint = _cameraBehavior.WindowOffset;
            _cameraBehavior.WindowSizeCheckpoint = _cameraBehavior.WindowSize;
            _cameraBehavior.PositionCheckpoint = _cameraBehavior.transform.position;
            _cameraBehavior.RotationCheckpoint = _cameraBehavior.transform.rotation;
        }
    }
}
