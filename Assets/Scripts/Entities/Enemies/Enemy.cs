using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using States;


public class Enemy : Entity
{
    private EntityMovement _entityController;

    [Header("Detection Zones")]
    [SerializeField] public DetectionZone CloseDetectionZone;
    [SerializeField] public DetectionZone FOVCone;
    [HideInInspector] public EnemyState State;
    [SerializeField] protected Weapon _weapon;

    [Header("Range Settings")]
    [Tooltip("The Distance to the player to kill him instant for cone, Indiced by purple line")]
    [SerializeField] private float DetectionRange;
    [Range(0, 1)] [SerializeField] private float _maxVibrationIntensity = 0.5f;
    [SerializeField] private float _distanceVibration = 10;

    [Header("Gauge Settings")]
    [SerializeField] public float DetectionTime = 100f;
    [SerializeField] private float GaugeAdd = 25;
    [SerializeField] private float GaugeRemove = 10;

    [HideInInspector] public bool PlayerDetected = false;
    [HideInInspector] public float TimeStamp = 0;
    protected PlayerStatus _player;

    private float _shootTime = 1f;
    public virtual EntityMovement Controller { get { return _entityController; } }

    // Start is called before the first frame update
    virtual public void Start()
    {
        _player = FindObjectOfType<PlayerStatus>();
        TimeStamp = DetectionTime;
        FOVCone.DistanceDetection = DetectionRange;
        if (_weapon == null)
            _weapon = new Weapon();
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
        if (!_player.Dead)  
        {

            if (TimeStamp > 0 && PlayerDetected)
            {
                TimeStamp -= Time.deltaTime * GaugeAdd;
                State = EnemyState.SUSPICIOUS;
            }
            else if (TimeStamp < DetectionTime)
            {
                TimeStamp += Time.deltaTime * GaugeRemove;
            }
            if (TimeStamp <= 0)
            {
                _player.Dead = true;
                Debug.Log("Shoot!!");
                Shoot();
                if (_weapon != null)
                    _weapon.Shoot();
                StartCoroutine(Shooting(0.5f));
            }
            SetVibrationController();
        }
    }

    virtual public void GoToPlayer(Vector3 lastPlayerPos) { }

    virtual public void Shoot() { }


    private IEnumerator Shooting(float time)
    {
        yield return new WaitForSeconds(time);
        EndShooting();
    }

    protected virtual void EndShooting()
    {
        PlayerDetected = false;
        TimeStamp = DetectionTime;
        State = EnemyState.NORMAL;
    }


    private void OnApplicationQuit()
    {
        if (Gamepad.current == null)
            return;
        Gamepad.current.SetMotorSpeeds(0, 0);
    }



    void SetVibrationController()
    {
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
            float vibrationIntensity = 1 - (distance) / (_distanceVibration);
            vibrationIntensity *= _maxVibrationIntensity;
            Gamepad.current.SetMotorSpeeds(vibrationIntensity, vibrationIntensity);
        }
    }
}