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
}

public class Entity : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected float speed;
}
