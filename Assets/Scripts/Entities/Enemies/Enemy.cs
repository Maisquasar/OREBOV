using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : Entity
{
    internal EntityMovement Controller;

    [Header("Detection Zones")]
    [SerializeField] public DetectionZone CloseDetectionZone;
    [SerializeField] public DetectionZone FOVCone;
    [Tooltip("The Distance to the player to kill him instant for cone, Indiced by purple line")]
    [Header("Range Settings")]
    [SerializeField] private float DetectionRange;
    [SerializeField] public float DetectionTime = 100f;
    [SerializeField] private float DistanceVibration = 10;
    [Header("Gauge Settings")]
    [SerializeField] private float GaugeAdd = 25;
    [SerializeField] private float GaugeRemove = 10;

    [HideInInspector] public bool PlayerDetected = false;
    [HideInInspector] public float TimeStamp = 0;

    protected PlayerStatus _player;
    // Start is called before the first frame update
    virtual public void Start()
    {
        _player = FindObjectOfType<PlayerStatus>();
        TimeStamp = DetectionTime;
        FOVCone.DistanceDetection = DetectionRange;
    }

    private void OnDrawGizmos()
    {
        if (Controller == null)
            return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + (Controller.Direction * DetectionRange * Vector3.left));
    }

    // Update is called once per frame
    virtual public void Update()
    {
        if (TimeStamp > 0 && PlayerDetected)
            TimeStamp -= Time.deltaTime * GaugeAdd;
        else if (TimeStamp < DetectionTime)
            TimeStamp += Time.deltaTime * GaugeRemove;
        if (TimeStamp <= 0)
            _player.Dead = true;

        // Set vibration intensity.
        if (Gamepad.current == null)
            return;
        if (_player.Dead)
            Gamepad.current.SetMotorSpeeds(0, 0);
        else
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            float vibrationIntensity = 1 - distance / DistanceVibration * 1;
            Gamepad.current.SetMotorSpeeds(vibrationIntensity, vibrationIntensity);
        }
    }

    private void OnApplicationQuit()
    {
        if (Gamepad.current == null)
            return;
        Gamepad.current.SetMotorSpeeds(0, 0);
    }
}
