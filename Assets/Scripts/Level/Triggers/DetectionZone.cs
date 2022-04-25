using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : Trigger
{
    public Enemy Enemy;
    private PlayerAnimator _playerAnimator;
    private PlayerStatus _playerStatus;
    [SerializeField] public bool IgnoreObstacles = false;
    [HideInInspector] public float DistanceDetection = 0;
    [HideInInspector] public bool LinkToLight = false;
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
            if (_playerAnimator == null)
            {
                GetPlayerComponents();
            }
            // All cases that player is not detected normaly :
            if ((CheckForObstacles() && !IgnoreObstacles)|| _playerAnimator.IsInAmination || _playerStatus.IsShadow || _playerStatus.IsHide)
            {
                // Case Obstacle between player and enemy then return.
                if (CheckForObstacles() && !IgnoreObstacles)
                {
                    return;
                }
                // Case Player is shadow.
                if (DistanceDetection > 0 && _playerStatus.MoveDir != Vector2.zero && _playerStatus.IsShadow)
                {
                    Enemy.PlayerDetected = true;
                    if (DistanceDetection != -1 && DistanceDetection >= Vector3.Distance(_playerStatus.transform.position, Enemy.transform.position))
                        Enemy.TimeStamp = 0;
                }
                // Case Player was detected.
                else if (Enemy.PlayerDetected)
                {
                    StartCoroutine(WaitForNextFrame());
                    Enemy.PlayerDetected = false;
                }
                return;
            }

            // Normal case :
            // If distance Equals 0, instant die, else decrement gauge.
            if (DistanceDetection == 0)
                Enemy.TimeStamp = 0;
            if (DistanceDetection != -1 && DistanceDetection >= Vector3.Distance(_playerStatus.transform.position, Enemy.transform.position))
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
            if (_playerStatus.IsShadow || _playerStatus.IsHide)
                return;
            StartCoroutine(WaitForNextFrame());
        }
    }

    private bool CheckForObstacles()
    {
        if (Enemy.Controller == null || LinkToLight)
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

    private void GetPlayerComponents()
    {
        _playerAnimator = FindObjectOfType<PlayerAnimator>();
        _playerStatus = _playerAnimator.GetComponent<PlayerStatus>();
    }
}
