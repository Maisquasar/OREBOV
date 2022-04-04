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
    [SerializeField] private bool airControl;
    [SerializeField] AnimationCurve velocityCurve;
    [HideInInspector] public PlayerAction PlayerActionState;
    [SerializeField] float jumpHeight;
    private float jumpForce;

    float margeDetectionVelocity = 0.05f;
    float time;

    private void Update()
    {
        if (rb.velocity.y < -margeDetectionVelocity)
        {
            ChangeStateFunction(ref PlayerActionState, PlayerAction.FALL);
        }
        else if (rb.velocity.y > margeDetectionVelocity)
        {
            ChangeStateFunction(ref PlayerActionState, PlayerAction.JUMP);
            animator.SetBool("Jump", true);
        }
        else if (rb.velocity.x < -margeDetectionVelocity || rb.velocity.x > margeDetectionVelocity)
        {
            ChangeStateFunction(ref PlayerActionState, PlayerAction.RUN);
            animator.SetBool("Jump", false);
        }
        else
        {
            ChangeStateFunction(ref PlayerActionState, PlayerAction.IDLE);
            animator.SetBool("Jump", false);
        }
        animator.SetFloat("VelocityX", rb.velocity.x);
    }

    public void Move(float move, bool jump)
    {
        move *= speed;
        if (grounded)
        {
            rb.velocity = new Vector2(velocityCurve.Evaluate(time) * move, rb.velocity.y);
        }
        if (grounded && jump)
        {
            grounded = false;
            jumpForce = Mathf.Sqrt(jumpHeight * -2 * (globalGravity * gravityScale));
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        time += Time.deltaTime;

        if ((move > 0 && direction == -1 || move < 0 && direction == 1) && (PlayerActionState == PlayerAction.IDLE || PlayerActionState == PlayerAction.RUN) && endOfCoroutine)
        {
            StartCoroutine(Flip(transform.rotation, transform.rotation * Quaternion.Euler(0, 180, 0), 0.1f));
        }
    }

    public void ChangeStateFunction<T>(ref T change, T state)
    {
        if (!change.Equals(state))
            change = state;
    }
}
