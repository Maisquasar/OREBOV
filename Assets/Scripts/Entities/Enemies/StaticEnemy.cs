using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : Enemy
{
    [SerializeField] public new StaticEnemyMovement Controller;

    Vector3 LastPlayerPosition;

    private void Update()
    {
        SetLastPlayerPos();
    }

    void SetLastPlayerPos()
    {
        if (PlayerDetected)
            LastPlayerPosition = _player.transform.position;
    }
}
