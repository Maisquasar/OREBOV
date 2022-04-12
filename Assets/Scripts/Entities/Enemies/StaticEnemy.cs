using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : Enemy
{
    [SerializeField] new public StaticEnemyMovement Controller;

    Vector3 LastPlayerPosition;

    override public void Update()
    {
        base.Update();
        SetLastPlayerPos();
        if (LastPlayerPosition.x > transform.position.x)
            Controller.Direction = 1;
        else
            Controller.Direction = -1;
        //Controller.Move(Controller.Direction);
    }

    void SetLastPlayerPos()
    {
        if (PlayerDetected)
            LastPlayerPosition = _player.transform.position;
    }
}
