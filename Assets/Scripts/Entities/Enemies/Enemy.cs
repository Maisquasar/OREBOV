using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : Entity
{
    private EntityMovement _entityController;

    [Header("Detection Zones")]
    [SerializeField] public DetectionZone CloseDetectionZone;
    [SerializeField] public DetectionZone FOVCone;

    [Tooltip("The Distance to the player to kill him instant for cone, Indiced by purple line")]
    [Header("Range Settings")]
    [SerializeField] private float DetectionRange;
    [SerializeField] public float DetectionTime = 100f;
    [Range(0, 1)] [SerializeField] private float _maxVibrationIntensity = 0.5f;
    [SerializeField] private float _distanceVibration = 10;
    [Header("Gauge Settings")]
    [SerializeField] private float GaugeAdd = 25;
    [SerializeField] private float GaugeRemove = 10;

    [HideInInspector] public bool PlayerDetected = false;
    [HideInInspector] public float TimeStamp = 0;
    protected PlayerStatus _player;

    public virtual EntityMovement Controller { get { return _entityController; } }

    // Start is called before the first frame update
    virtual public void Start()
    {
        _player = FindObjectOfType<PlayerStatus>();
        TimeStamp = DetectionTime;
        FOVCone.DistanceDetection = DetectionRange;
    }

    private void OnDrawGizmos()
    {
        if (_entityController == null)
            return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + (_entityController.Direction * DetectionRange * Vector3.left));
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
        if (_player == null)
            return;
        if (_player.Dead)
            Gamepad.current.SetMotorSpeeds(0, 0);
        else
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            float vibrationIntensity = 1 - (distance ) / (_distanceVibration);
            vibrationIntensity *= _maxVibrationIntensity;
            Gamepad.current.SetMotorSpeeds(vibrationIntensity, vibrationIntensity);
        }
    }

    virtual public void GoToPlayer(Vector3 lastPlayerPos) { }

    private void OnApplicationQuit()
    {
        if (Gamepad.current == null)
            return;
        Gamepad.current.SetMotorSpeeds(0, 0);
    }
}
