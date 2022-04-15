using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckpoint : MonoBehaviour
{
    [Tooltip("Time in seconds")]
    [SerializeField] public float Time;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
    