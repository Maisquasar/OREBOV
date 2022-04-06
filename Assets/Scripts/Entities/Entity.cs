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
        HIDE,
        PUSHING,
        INTERACT,
        CLIMB,
        DEAD
    }

    public enum EnemyState
    { 
        NOT_DETECTED,
        SUSPICIOUS,
        ALERT,
        CHASE
    }

}

public class Entity : MonoBehaviour
{
}
