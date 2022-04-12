using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCheckpointManager))]
public class MobileEnemy : Enemy
{
    [SerializeField] new public MobileEnemyMovement Controller;

    [Header("Checkpoint Manager")]  [Space]
    [SerializeField] EnemyCheckpointManager _checkpointManager;
    int currentCheckpoint;

    override public void Start()
    {
        base.Start();
        Controller.NewCheckpoint(_checkpointManager.Checkpoints[currentCheckpoint].transform.position);
    }

    // Update is called once per frame
    override public void Update()
    {
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
        if (!decrease)
        {
            if (currentCheckpoint < _checkpointManager.Checkpoints.Count - 1)
                currentCheckpoint++;
            else
                decrease = true;
        }
        else
        {
            if (currentCheckpoint > 0)
                currentCheckpoint--;
            else
                decrease = false;
        }
        Controller.NewCheckpoint(_checkpointManager.Checkpoints[currentCheckpoint].transform.position);
    }

    bool stillWaiting = false;
    IEnumerator WaitCheckpoint()
    {
        int indexAtStart = currentCheckpoint;
        stillWaiting = true;
        Debug.Log($"Wait for {_checkpointManager.Checkpoints[indexAtStart].Time} seconds");
        yield return new WaitForSeconds(_checkpointManager.Checkpoints[indexAtStart].Time);
        stillWaiting = false;
    }
}
