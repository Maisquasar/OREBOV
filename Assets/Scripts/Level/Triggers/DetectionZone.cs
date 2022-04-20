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
        Component t = other.gameObject.GetComponent(typeof(PlayerStatus));
        if (t != null)
        {
            if (CheckForObstacles() || _playerAnimator.IsInAmination || _playerStatus.IsShadow || _playerStatus.IsHide)
            {
                return;
            }
            if (DistanceDetection == 0)
                Enemy.TimeStamp = 0;
            if (DistanceDetection >= Vector3.Distance(_playerStatus.transform.position, Enemy.transform.position))
                Enemy.TimeStamp = 0;
            Enemy.PlayerDetected = true;
            ((PlayerStatus)t).StressPlayer(5.0f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStatus>())
        {
            Enemy.PlayerDetected = false;
            StartCoroutine(WaitForNextFrame());
        }
    }

    private bool CheckForObstacles()
    {
        if (Enemy.Controller == null)
            return false;
        if (Physics.Raycast(Enemy.transform.position, _playerStatus.transform.position - Enemy.transform.position, Vector3.Distance(Enemy.transform.position, _playerStatus.transform.position), Enemy.Controller.WallType, QueryTriggerInteraction.Ignore))
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
