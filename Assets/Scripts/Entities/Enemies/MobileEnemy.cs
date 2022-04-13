using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCheckpointManager))]
public class MobileEnemy : Enemy
{
    [Header("Controller")]
    [SerializeField] new public MobileEnemyMovement Controller;

    [Header("Checkpoint Manager")]
    [Space]
    EnemyCheckpointManager _checkpointManager;
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
            if ((int)transform.position.x == (int)_checkpointManager.Checkpoints[currentCheckpoint].transform.position.x)
            {
                CheckpointChange();
            }
        }
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

    bool stillWaiting = false;
    IEnumerator WaitCheckpoint()
    {
        int indexAtStart = currentCheckpoint;
        stillWaiting = true;
        yield return new WaitForSeconds(_checkpointManager.Checkpoints[indexAtStart].Time);
        stillWaiting = false;
    }
}
