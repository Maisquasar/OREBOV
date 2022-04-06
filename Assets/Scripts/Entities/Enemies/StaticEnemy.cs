using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : Entity
{
    [SerializeField] public StaticEnemyMovement Controller;
    [HideInInspector] public DetectionZone DetectionZone;

    private Player player;
    [SerializeField] private float _detectionTime = 10f;
    private float _timeStamp = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        DetectionZone = GetComponentInChildren<DetectionZone>();
        _timeStamp = _detectionTime;
    }

    // Update is called once per frame
    void Update()
    {
        // If in zone decrease time stamp,
        if (DetectionZone.Detect && _timeStamp > 0)
            _timeStamp -= Time.deltaTime;
        // else is not in zone so increase time stamp,
        else if (_timeStamp < _detectionTime)
            _timeStamp += Time.deltaTime;
        // if time stamp < 0 then kill player.
        if (_timeStamp <= 0)
            player.Controller.SetDead();

        Debug.Log((_timeStamp / _detectionTime) * 1);
    }

}
