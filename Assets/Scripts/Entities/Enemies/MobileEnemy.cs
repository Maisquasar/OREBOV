using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCheckpointManager))]
public class MobileEnemy : Enemy
{
    [Header("Controller")]
    [HideInInspector] public MobileEnemyMovement _controller;

    [Tooltip("The time the enemy search the player")]
    [SerializeField] float SearchTime = 1f;
    EnemyCheckpointManager _checkpointManager;
    bool _followPlayer = false;
    int _currentCheckpoint;

    float _timeStamp;
    float _secondCheckStuck = 2f;
    Vector3 _precPoS;

    public override EntityMovement Controller { get { return _controller; } }

    override public void Start()
    {
        _controller = GetComponent<MobileEnemyMovement>();
        _currentCheckpoint = 0;
        StartCoroutine(WaitStart());
    }

    IEnumerator WaitStart()
    {
        yield return new WaitUntil(() => _checkpointManager != null);
        base.Start();
    }

    public void SetCheckpointManager(EnemyCheckpointManager manage)
    {
        _checkpointManager = manage;
    }

    // Update is called once per frame
    override public void Update()
    {
        if (_checkpointManager == null)
        {
            Debug.LogError("Missing Checkpoint Manager");
            return;
        }
        base.Update();
        if (!stillWaiting)
        {
            _controller.Move(_controller.Direction);
            if (!_followPlayer && (int)transform.position.x == (int)_checkpointManager.Checkpoints[_currentCheckpoint].transform.position.x)
            {
                CheckpointChange();
            }
            else if (_followPlayer && (int)transform.position.x == (int)lastPlayerPos.x)
            {
                StopFollowingPlayer();
            }
        }


        // Check if same position every {_secondCheckStuck} in seconds.
        if (_timeStamp <= Time.time)
        {
            if (_precPoS == (Vector3)transform.position && _followPlayer)
            {
                StopFollowingPlayer();
            }
            _precPoS = transform.position;
            _timeStamp = Time.time + _secondCheckStuck;
        }
    }

    void StopFollowingPlayer()
    {
        Debug.Log("Stop following Player");
        _followPlayer = false;
        StartCoroutine(WaitPlayerSearch());
    }

    Vector3 lastPlayerPos;
    override public void GoToPlayer(Vector3 lastPlayerPos) 
    {
        this.lastPlayerPos = lastPlayerPos;
        Debug.Log("Follow Player");
        _followPlayer = true;
        _controller.NewCheckpoint(lastPlayerPos);
    }

    bool decrease = false;
    public void CheckpointChange()
    {
        StartCoroutine(WaitCheckpoint());
        if (!_checkpointManager.Reverse)
        {
            if (_currentCheckpoint < _checkpointManager.Checkpoints.Count - 1)
                _currentCheckpoint++;
            else
                _currentCheckpoint = 0;
        }
        else
        {
            if (decrease)
            {
                if (_currentCheckpoint > 0)
                {
                    _currentCheckpoint--;
                }
                else
                {
                    _currentCheckpoint++;
                    decrease = false;
                }
            }
            else
            {
                if (_currentCheckpoint < _checkpointManager.Checkpoints.Count - 1)
                {
                    _currentCheckpoint++;
                }
                else
                {
                    _currentCheckpoint--;
                    decrease = true;
                }
            }
        }
        _controller.NewCheckpoint(_checkpointManager.Checkpoints[_currentCheckpoint].transform.position);
    }

    IEnumerator WaitPlayerSearch()
    {
        stillWaiting = true;
        yield return new WaitForSeconds(SearchTime);
        _controller.NewCheckpoint(_checkpointManager.Checkpoints[_currentCheckpoint].transform.position);
        stillWaiting = false;
    }

    bool stillWaiting = false;
    IEnumerator WaitCheckpoint()
    {
        int indexAtStart = _currentCheckpoint;
        stillWaiting = true;
        yield return new WaitForSeconds(_checkpointManager.Checkpoints[indexAtStart].Time);
        stillWaiting = false;
    }
}
