using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;


namespace States
{
    public enum PlayerAction
    {
        IDLE,
        RUN,
        JUMP,
        FALL,
        HIDE,
        INTERACT,
        CLIMB,
        DEAD
    }
}
public class PlayerMovement : EntityMovement
{
    [SerializeField] private float jumpForce;
    [SerializeField] private bool airControl;
    [SerializeField] AnimationCurve velocityCurve;
    [HideInInspector] public PlayerAction PlayerActionState;

    float margeDetectionVelocity = 0.2f;
    float time;

    private void Update()
    {
        if (rb.velocity.y < -margeDetectionVelocity)
            ChangeStateFunction(ref PlayerActionState, PlayerAction.FALL);
        else if (rb.velocity.y > margeDetectionVelocity)
            ChangeStateFunction(ref PlayerActionState, PlayerAction.JUMP);
        else if (rb.velocity.x < -margeDetectionVelocity || rb.velocity.x > margeDetectionVelocity)
            ChangeStateFunction(ref PlayerActionState, PlayerAction.RUN);
        else
            ChangeStateFunction(ref PlayerActionState, PlayerAction.IDLE);
    }

    public void Move(float move, bool jump)
    {
        if (grounded || airControl)
        {
            rb.velocity = new Vector2(velocityCurve.Evaluate(time) * move, rb.velocity.y);
        }
        if (grounded && jump)
        {
            grounded = false;
            rb.AddForce((Vector3.up * (jumpForce) * 10));
        }
        time += Time.deltaTime;
    }

    public void ChangeStateFunction<T>(ref T change, T state)
    {
        if (!change.Equals(state))
            change = state;
    }
}
