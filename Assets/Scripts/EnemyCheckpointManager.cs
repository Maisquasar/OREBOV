using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCheckpointManager : MonoBehaviour
{
    [SerializeField] public List<EnemyCheckpoint> Checkpoints = new List<EnemyCheckpoint>();
    [SerializeField] public MobileEnemy _enemy;
    [SerializeField] public float StartTime;
    [SerializeField] public bool Reverse;

    // Start is called before the first frame update
    void Start()
    {
        if (_enemy == null)
            return;
        _enemy.SetCheckpointManager(this);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            if (Checkpoints[i] == null)
                return;
            Checkpoints[i].transform.position = new Vector3(Checkpoints[i].transform.position.x, _enemy.transform.position.y, _enemy.transform.position.z);
            Gizmos.color = Color.yellow;
            if (i != 0)
                Gizmos.DrawLine(Checkpoints[i - 1].transform.position, Checkpoints[i].transform.position);
            else if (i == 0)
                Gizmos.DrawLine(Checkpoints[i].transform.position, Checkpoints[Checkpoints.Count - 1].transform.position);
            Gizmos.color = Color.clear;
        }
    }

    public void DetectCheckpoints()
    {
        if (transform.childCount == 0)
        {
            Debug.LogError("No Checkpoint found");
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.name == "Enemy Position")
                DestroyImmediate(transform.GetChild(i).gameObject);
        }


        Checkpoints.Clear();
        Checkpoints.Add(Instantiate(transform.GetChild(0).GetComponent<EnemyCheckpoint>(), _enemy.transform.position, _enemy.transform.rotation, transform));
        Checkpoints.Last().Time = StartTime;
        Checkpoints.Last().gameObject.name = "Enemy Position";
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.name != "Enemy Position")
                Checkpoints.Add(transform.GetChild(i).GetComponent<EnemyCheckpoint>());
        }
    }
}
