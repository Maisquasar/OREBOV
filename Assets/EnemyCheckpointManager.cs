using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCheckpointManager : MonoBehaviour
{
    public List<EnemyCheckpoint> Checkpoints = new List<EnemyCheckpoint>();
    [SerializeField] Enemy _enemy;
    [SerializeField] float StartTime;
    // Start is called before the first frame update
    void Start()
    {
        Checkpoints.Add(Instantiate(transform.GetChild(0).GetComponent<EnemyCheckpoint>(), _enemy.transform.position, _enemy.transform.rotation, transform));
        Checkpoints.Last().Time = StartTime;
        for (int i = 0; i < transform.childCount; i++)
        {
            Checkpoints.Add(transform.GetChild(i).GetComponent<EnemyCheckpoint>());
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.color = Color.yellow;
            if (i != 0)
                Gizmos.DrawLine(transform.GetChild(i - 1).transform.position, transform.GetChild(i).transform.position);
            else if (i == 0 && _enemy != null)
                Gizmos.DrawLine(transform.GetChild(i).transform.position, _enemy.transform.position);
            Gizmos.color = Color.clear;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
