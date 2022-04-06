using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;

public class PlayerMovement : EntityMovement
{
    [SerializeField] private bool airControl;
    [SerializeField] AnimationCurve velocityCurve;
    [SerializeField] float jumpHeight;
    [SerializeField] private float jumpDistance;

    float edgeDetectorDistance = 0.5f;
    float topEdgeDetectorDistance = 0.75f;

    private float lastMove;
    private float jumpForce;

    float margeDetectionVelocity = 0.05f;
    float time;

    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0, edgeDetectorDistance, 0), transform.position + new Vector3(1, edgeDetectorDistance, 0));
        Gizmos.DrawLine(transform.position + new Vector3(0, edgeDetectorDistance, 0), transform.position + new Vector3(-1, edgeDetectorDistance, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + new Vector3(0, topEdgeDetectorDistance, 0), transform.position + new Vector3(1, topEdgeDetectorDistance, 0));
        Gizmos.DrawLine(transform.position + new Vector3(0, topEdgeDetectorDistance, 0), transform.position + new Vector3(-1, topEdgeDetectorDistance, 0));
    }

    Vector3 posAtClimb;
    private void Update()
    {
        if (!DetectWall())
            animator.SetFloat("VelocityX", rb.velocity.x);
        else
            animator.SetFloat("VelocityX", 0);
        animator.SetFloat("VelocityY", rb.velocity.y);
        animator.SetBool("Grounded", grounded);

        for (int i = 0; i < 2; i++)
        {
            RaycastHit[] topRay = Physics.RaycastAll(transform.position + new Vector3(0, topEdgeDetectorDistance, 0), i == 0 ? Vector3.right : Vector3.left, 1);
            RaycastHit[] downRay = Physics.RaycastAll(transform.position + new Vector3(0, edgeDetectorDistance, 0), i == 0 ? Vector3.right : Vector3.left, 1);
            foreach (var ray in downRay)
            {
                if (ray.distance < 1 && topRay.Length == 0)
                {
                    rb.velocity = Vector3.zero;
                    gravityScale = 0;
                    StartCoroutine(PlayClimb());
                }
            }
        }
    }

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
        if (State == PlayerAction.RUN)
            animator.speed = Mathf.Abs(lastMove);
        else
            animator.speed = 1;
    }

    public void Move(float move, bool jump)
    {
        lastMove = move;
        if (rb.velocity.y < 0.1f)
            move *= speed;
        else if (move != 0)
            move = jumpDistance * speed * Mathf.Sign(move);

        if (grounded && !jump)
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

        if ((move > 0 && direction == -1 || move < 0 && direction == 1) && grounded && endOfCoroutine)
        {
            StartCoroutine(Flip(transform.rotation, transform.rotation * Quaternion.Euler(0, 180, 0), 0.1f));
        }
    }

    public void ChangeStateFunction<T>(ref T change, T state)
    {
        if (!change.Equals(state))
            change = state;
    }

    int endOfAnim = -1;
    public IEnumerator PlayClimb()
    {
        endOfAnim = 0;
        animator.Play("Climb");
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(animationLength);
        //transform.position = transform.position + new Vector3()
    }
}

