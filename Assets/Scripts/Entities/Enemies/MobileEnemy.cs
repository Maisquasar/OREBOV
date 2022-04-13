using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCheckpointManager))]
public class MobileEnemy : Enemy
{
    [Header("Controller")]
    [SerializeField] new public MobileEnemyMovement Controller;

    [Tooltip("The time the enemy search the player")]
    [SerializeField] float SearchTime = 1f;
    EnemyCheckpointManager _checkpointManager;
    bool followPlayer = false;
    int currentCheckpoint;

    override public void Start()
    {
        currentCheckpoint = 0;
        StartCoroutine(WaitStart());
    }

    IEnumerator WaitStart()
    {
        yield return new WaitUntil(() => _checkpointManager != null);
        base.Start();
        //CheckpointChange();
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
            Controller.Move(Controller.Direction);
            if (!followPlayer && (int)transform.position.x == (int)_checkpointManager.Checkpoints[currentCheckpoint].transform.position.x)
            {
                CheckpointChange();
            }
            else if (followPlayer && (int)transform.position.x == (int)lastPlayerPos.x)
            {
                followPlayer = false;
                StartCoroutine(WaitPlayerSearch());
            }
        }
    }

    Vector3 lastPlayerPos;
    override public void GoToPlayer(Vector3 lastPlayerPos) 
    {
        this.lastPlayerPos = lastPlayerPos;
        Debug.Log("Follow Player");
        followPlayer = true;
        Controller.NewCheckpoint(lastPlayerPos);
    }

    bool decrease = false;
    public void CheckpointChange()
    {
        StartCoroutine(WaitCheckpoint());
        if (!_checkpointManager.Reverse)
        {
            if (currentCheckpoint < _checkpointManager.Checkpoints.Count - 1)
                currentCheckpoint++;
            else
                currentCheckpoint = 0;
        }
        else
        {
            if (decrease)
            {
                if (currentCheckpoint > 0)
                {
                    currentCheckpoint--;
                }
                else
                {
                    currentCheckpoint++;
                    decrease = false;
                }
            }
            else
            {
                if (currentCheckpoint < _checkpointManager.Checkpoints.Count - 1)
                {
                    currentCheckpoint++;
                }
                else
                {
                    currentCheckpoint--;
                    decrease = true;
                }
            }
        }
        Controller.NewCheckpoint(_checkpointManager.Checkpoints[currentCheckpoint].transform.position);
    }

    IEnumerator WaitPlayerSearch()
    {
        stillWaiting = true;
        yield return new WaitForSeconds(SearchTime);
        Controller.NewCheckpoint(_checkpointManager.Checkpoints[currentCheckpoint].transform.position);
        stillWaiting = false;
    }

    bool stillWaiting = false;
    IEnumerator WaitCheckpoint()
    {
        int indexAtStart = currentCheckpoint;
        stillWaiting = true;
        yield return new WaitForSeconds(_checkpointManager.Checkpoints[indexAtStart].Time);
        stillWaiting = false;
    }
}
