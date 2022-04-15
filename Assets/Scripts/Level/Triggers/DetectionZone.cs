using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : Trigger
{
    public Enemy Enemy;
    private PlayerAnimator _playerAnimator;
    private PlayerStatus _playerStatus;
    [HideInInspector] public float DistanceDetection = 0;

    public override void Start()
    {
        base.Start();
        _playerAnimator = FindObjectOfType<PlayerAnimator>();
        _playerStatus = _playerAnimator.GetComponent<PlayerStatus>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>())
        {
            if (CheckForObstacles() || _playerAnimator.IsInAmination)
                return;
            if (Enemy.PlayerDetected && _playerStatus.IsShadow)
            {
                Enemy.PlayerDetected = true;
            }
            else if (_playerStatus.IsShadow)
                return;

            if (DistanceDetection == 0)
                Enemy.TimeStamp = 0;
            Enemy.PlayerDetected = true;
                
            if (DistanceDetection >= Vector3.Distance(_playerStatus.transform.position, Enemy.transform.position))
                Enemy.TimeStamp = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>())
        {
            if (CheckForObstacles() || _playerStatus.IsShadow)
                return;
            Enemy.PlayerDetected = false;
            StartCoroutine(WaitForNextFrame());
        }
    }

    private bool CheckForObstacles()
    {
        if (Enemy.Controller == null)
            return false;
        if (Physics.Raycast(Enemy.transform.position, Vector3.right * Enemy.Controller.Direction, Vector3.Distance(Enemy.transform.position, _playerStatus.transform.position), Enemy.Controller.GroundType, QueryTriggerInteraction.Ignore))
            return true;
        return false;
    }

    IEnumerator WaitForNextFrame()
    {
        yield return new WaitForEndOfFrame();
        if (!Enemy.PlayerDetected)
            Enemy.GoToPlayer(_playerStatus.transform.position);
    }
}
