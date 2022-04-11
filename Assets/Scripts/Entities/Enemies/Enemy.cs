using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : Entity
{
    protected EntityMovement Controller;
    [SerializeField] public DetectionZone CloseDetectionZone;
    [SerializeField] public DetectionZone FOVCone;
    [Tooltip("The Distance to the player to kill him insant for cone, Indiced by purple line")]
    [SerializeField] private float DetectionRange;
    [SerializeField] public float DetectionTime = 100f;
    [SerializeField] private float DistanceVibration = 10;
    [SerializeField] private float GaugeAdd = 25;
    [SerializeField] private float GaugeRemove = 10;

    [HideInInspector] public bool PlayerDetected = false;
    [HideInInspector] public float TimeStamp = 0;

    Vector3 StartPos;
    protected Player _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
        TimeStamp = DetectionTime;
        FOVCone.DistanceDetection = DetectionRange;
        StartPos = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + (Controller.Direction * DetectionRange * Vector3.left));
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeStamp > 0 && PlayerDetected)
            TimeStamp -= Time.deltaTime * GaugeAdd;
        else if (TimeStamp < DetectionTime)
            TimeStamp += Time.deltaTime * GaugeRemove;
        if (TimeStamp <= 0)
            _player.Controller.SetDead();

        // Set vibration intensity.
        if (_player.Dead)
            Gamepad.current.SetMotorSpeeds(0, 0);
        else
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            float vibrationIntensity = 1 - distance / DistanceVibration * 1;
            Gamepad.current.SetMotorSpeeds(vibrationIntensity, vibrationIntensity);
        }
    }

}
