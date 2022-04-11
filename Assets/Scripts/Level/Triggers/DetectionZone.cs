using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : Trigger
{
    public UnityEvent DetectedEvent;
    public StaticEnemy Enemy;
    private Player _player;
    [HideInInspector] public float DistanceDetection = 0;

    public override void Start()
    {
        _player = FindObjectOfType<Player>();
        base.Start();
        if (DetectedEvent == null)
        {
            DetectedEvent.AddListener(LogDetected);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            if (CheckForObstacles())
                return;
            Enemy.PlayerDetected = true;
            if (DistanceDetection > Vector3.Distance(_player.transform.position, Enemy.transform.position))
                Enemy.TimeStamp = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            Enemy.PlayerDetected = false;
        }
    }


    private bool CheckForObstacles()
    {
        if (Physics.Raycast(Enemy.transform.position, Vector3.right * Enemy.Controller.Direction, Vector3.Distance(Enemy.transform.position, _player.transform.position) - 0.1f, Enemy.Controller.GroundType, QueryTriggerInteraction.Ignore))
            return true;
        return false;
    }


    public static void LogDetected()
    {
        Debug.Log("Detected");
    }
}
