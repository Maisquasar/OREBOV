using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
    public enum PlayerAction
    {
        IDLE,
        RUN,
        JUMP,
        FALL,
        PUSHING,
        INTERACT,
        CLIMB,
        DEAD
    }

    public enum EnemyState
    { 
        NORMAL,
        SUSPICIOUS,
        INTERACT,
        ALERT,
        CHASE,
        INTERACT,
    }
}

public class Entity : MonoBehaviour
{
}
