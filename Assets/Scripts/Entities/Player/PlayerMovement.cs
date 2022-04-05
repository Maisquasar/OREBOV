using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;


public class PlayerMovement : EntityMovement
{
    [SerializeField] private bool airControl;
    [SerializeField] AnimationCurve velocityCurve;
    [HideInInspector] public PlayerAction PlayerActionState;
    [SerializeField] float jumpHeight;
    [SerializeField] private float jumpDistance;

    float edgeDetectorDistance = 0.5f;
    float topEdgeDetectorDistance = 0.75f;

    private float lastMove;
    private float jumpForce;

    float margeDetectionVelocity = 0.05f;
    float time;

    private new void Start()
    {
        base.Start();
        Physics.Raycast(transform.position + new Vector3(0, edgeDetectorDistance, 0), Vector3.right, 1);
        Physics.Raycast(transform.position + new Vector3(0, edgeDetectorDistance, 0), Vector3.left, 1);
    }

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

    private void Update()
    {
        if (rb.velocity.y < -margeDetectionVelocity)
        {
            ChangeStateFunction(ref PlayerActionState, PlayerAction.FALL);
        }
        else if (rb.velocity.y > margeDetectionVelocity)
        {
            ChangeStateFunction(ref PlayerActionState, PlayerAction.JUMP);
        }
        else if (rb.velocity.x < -margeDetectionVelocity || rb.velocity.x > margeDetectionVelocity)
        {
            ChangeStateFunction(ref PlayerActionState, PlayerAction.RUN);
        }
        else
        {
            ChangeStateFunction(ref PlayerActionState, PlayerAction.IDLE);
        }
        if (PlayerActionState == PlayerAction.RUN)
            animator.speed = Mathf.Abs(lastMove);
        else
            animator.speed = 1;

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
                    Debug.Log($"Edge");
                }
            }
        }

    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();

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

