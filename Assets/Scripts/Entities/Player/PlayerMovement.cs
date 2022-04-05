using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;

public class PlayerMovement : EntityMovement
{
    [SerializeField] private float jumpForce;
    [SerializeField] private bool airControl;
    [SerializeField] AnimationCurve velocityCurve;

    float margeDetectionVelocity = 0.2f;
    float time;

    public void ChangeState(ref PlayerAction State)
    {
        if (State != PlayerAction.INTERACT && State != PlayerAction.PUSHING)
        {
            if (rb.velocity.y < -margeDetectionVelocity)
                ChangeStateFunction(ref State, PlayerAction.FALL);
            else if (rb.velocity.y > margeDetectionVelocity)
                ChangeStateFunction(ref State, PlayerAction.JUMP);
            else if (rb.velocity.x < -margeDetectionVelocity || rb.velocity.x > margeDetectionVelocity)
                ChangeStateFunction(ref State, PlayerAction.RUN);
            else
                ChangeStateFunction(ref State, PlayerAction.IDLE);
        }
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
            rb.AddForce(jumpForce * 10 * Vector3.up);
        }
        time += Time.deltaTime;
    }

    public void ChangeStateFunction<T>(ref T change, T state)
    {
        if (!change.Equals(state))
            change = state;
    }
}
