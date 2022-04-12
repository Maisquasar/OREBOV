using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : Trigger
{
    public Enemy Enemy;
    private Player _player;
    [HideInInspector] public float DistanceDetection = 0;

    public override void Start()
    {
        base.Start();
        _player = FindObjectOfType<Player>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            if (CheckForObstacles() || _player.IsShadow)
                return;
            Enemy.PlayerDetected = true;
            if (DistanceDetection >= Vector3.Distance(_player.transform.position, Enemy.transform.position))
                Enemy.TimeStamp = 0;
            else if (DistanceDetection == 0)
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
        if (Enemy.Controller == null)
            return false;
        if (Physics.Raycast(Enemy.transform.position, Vector3.right * Enemy.Controller.Direction, Vector3.Distance(Enemy.transform.position, _player.transform.position), Enemy.Controller.GroundType, QueryTriggerInteraction.Ignore))
            return true;
        return false;
    }
}
