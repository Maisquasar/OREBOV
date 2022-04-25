using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Trigger
{
    private bool _set = false;
    private CameraBehavior _camera;

    [HideInInspector] public Vector3 Position;

    [HideInInspector] public Vector3 CamPos;
    [HideInInspector] public Quaternion CamRot;
    [HideInInspector] public Vector2 CamOffset;
    [HideInInspector] public Vector2 CamSize;

    public override void Start()
    {
        base.Start();
        Camera.main.GetComponent<CameraBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerStatus>() && !_set)
        {
            _set = true;
            _camera = Camera.main.GetComponent<CameraBehavior>();
            Position = transform.position + Vector3.up;
            CamOffset = _camera.WindowOffset;
            CamSize = _camera.WindowSize;
            CamPos = _camera.transform.position;
            CamRot = _camera.transform.rotation;
            other.GetComponent<PlayerStatus>().LastCheckpoint = this;
        }
    }
}
