using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;
using UnityEditor;

public class PlayerMovement : EntityMovement
{
    [SerializeField] AnimationCurve velocityCurve;

    [Space]
    [Header("Jump Settings")]
    [Space]
    [SerializeField] float jumpHeight;
    [SerializeField] private float jumpDistance;

    [Space]
    [Header("Edge Detector Settings")]
    [Space]
    [SerializeField] float edgeDetectorHeight = 0.6f;
    private float topEdgeDetectorHeight = 0.75f;
    float edgeDetectorDistance = 0.5f;

    [Space]
    [Header("Fall Damage Settings")]
    [Space]
    [SerializeField] float FallDamageHeight = 8;

    private float lastMove;
    private float jumpForce;

    [Header("Sounds ")]
    [SerializeField]
    private SoundEffectsHandler _walkEffectsHandler;
    [SerializeField]
    private SoundEffectsHandler _jumpImpactEffectHandler;

    private float _xAxisValue;

    float margeDetectionVelocity = 0.05f;
    float time;


    private new void Start()
    {
        canTurn = true;
        base.Start();
        gravityScale = 3;
        topEdgeDetectorHeight = edgeDetectorHeight + 0.15f;

    }

    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Draw edge Detector
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0, edgeDetectorHeight, 0), transform.position + new Vector3(edgeDetectorDistance, edgeDetectorHeight, 0));
        Gizmos.DrawLine(transform.position + new Vector3(0, edgeDetectorHeight, 0), transform.position + new Vector3(-edgeDetectorDistance, edgeDetectorHeight, 0));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + new Vector3(0, topEdgeDetectorHeight, 0), transform.position + new Vector3(edgeDetectorDistance, topEdgeDetectorHeight, 0));
        Gizmos.DrawLine(transform.position + new Vector3(0, topEdgeDetectorHeight, 0), transform.position + new Vector3(-edgeDetectorDistance, topEdgeDetectorHeight, 0));
    }

    [HideInInspector] public bool isClimbing = false;
    bool FallDefine = false;

    private void Update()
    {
        //Get the pos at start fall.
        if (rb.velocity.y < -0.1f && !FallDefine && !grounded)
        {
            Debug.Log("Define");
            LastPosBeforeFall = transform.position;
            FallDefine = true;
        }
        //Check if fall damage.
        if (grounded)
        {
            FallDefine = false;
            if (LastPosBeforeFall != null && LastPosBeforeFall.y - transform.position.y >= GameMetric.GetGameUnit(FallDamageHeight) && !GetComponent<Player>().Dead)
            {
                animator.SetBool("Dead", true);
                GetComponent<Player>().Dead = true;
                LastPosBeforeFall = GetComponent<Player>().CheckpointPos;
            }
        }

        if (!DetectWall())
            animator.SetFloat("VelocityX", rb.velocity.x);
        else
            animator.SetFloat("VelocityX", 0);
        animator.SetFloat("VelocityY", rb.velocity.y);
        animator.SetBool("Grounded", grounded);

        // Can't climb if fall damage.
        if (LastPosBeforeFall.y - transform.position.y < GameMetric.GetGameUnit(FallDamageHeight))
        {
            // Edge Detection :
            RaycastHit[] topRay = Physics.RaycastAll(transform.position + new Vector3(0, topEdgeDetectorHeight, 0), Vector3.right * direction, edgeDetectorDistance, GroundType, QueryTriggerInteraction.Ignore);
            RaycastHit[] downRay = Physics.RaycastAll(transform.position + new Vector3(0, edgeDetectorHeight, 0), Vector3.right * direction, edgeDetectorDistance, GroundType, QueryTriggerInteraction.Ignore);
            foreach (var ray in downRay)
            {
                if (topRay.Length == 0 && !isClimbing)
                {
                    StartCoroutine(PlayClimb());
                }
            }
        }
    }
    Vector3 LastPosBeforeFall;
    public void ChangeState(ref PlayerAction State)
    {
        if (State != PlayerAction.INTERACT && State != PlayerAction.PUSHING)
        {
            if (rb.velocity.y < -margeDetectionVelocity)
            {
                ChangeStateFunction(ref State, PlayerAction.FALL);
            }
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

    // Move the player.
    public void Move(float move, bool jump)
    {
        _xAxisValue = move;
        // If climbing then can't move
        if (isClimbing || isPushing || isPulling)
            return;
        lastMove = move;
        if (rb.velocity.y < 0.1f)
            move *= speed;
        else if (move != 0)
            move = jumpDistance * speed * Mathf.Sign(move);

        // Ground Move
        if (grounded && !jump)
        {

            rb.velocity = new Vector2(velocityCurve.Evaluate(time) * move, rb.velocity.y);
        }
       
        // Jump move
        if (grounded && jump)
        {
           // grounded = false;
            jumpForce = Mathf.Sqrt(jumpHeight * -2 * (globalGravity * gravityScale));
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        //Flip character
        if ((move > 0 && direction == -1 || move < 0 && direction == 1) && grounded && endOfCoroutine)
        {
            StartCoroutine(Flip(transform.rotation, transform.rotation * Quaternion.Euler(0, 180, 0), 0.1f));
        }
        time += Time.deltaTime;
    }

    public void ChangeStateFunction<T>(ref T change, T state)
    {
        if (!change.Equals(state))
            change = state;
    }

    public void FlipCharacter()
    {
        if (transform.rotation.eulerAngles.y == 180f)
            StartCoroutine(Flip(transform.rotation, transform.rotation * Quaternion.Euler(0, -180, 0), 0.1f));
        else if (transform.rotation.eulerAngles.y == 0)
            StartCoroutine(Flip(transform.rotation, transform.rotation * Quaternion.Euler(0, 180, 0), 0.1f));
    }

    protected override void LandOnGround()
    {
        base.LandOnGround();
        _jumpImpactEffectHandler.PlaySound();
    }

    public IEnumerator PlayClimb()
    {
        RaycastHit[] downRay = Physics.RaycastAll(transform.position + new Vector3(0, edgeDetectorHeight, 0), Vector3.right * direction, edgeDetectorDistance, GroundType, QueryTriggerInteraction.Ignore);
        StartCoroutine(LerpTo(transform.position + Vector3.right * direction * (downRay[0].distance - 0.4f), 0.1f));
        isClimbing = true;
        //Play animation
        animator.Play("Climb");
        //Lock Player pos
        rb.velocity = Vector3.zero;
        gravityScale = 0;
        // Wait for end of animation
        yield return new WaitForSecondsRealtime(1.02f);
        // Move to animation pos.
        transform.position = transform.position + new Vector3(0.4f * direction, 1.7f, 0);
        // Reset Grabity scale.
        gravityScale = 3;
        // Transition to Idle.
        yield return new WaitForSeconds(0.75f);
        isClimbing = false;
    }

    IEnumerator LerpTo(Vector3 goTo, float duration)
    {
        Vector3 initial = transform.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        transform.position = goTo;
    }

    [HideInInspector] public bool isPushing = false;
    public IEnumerator PlayPush()
    {
        if (!isPushing)
        {
            isPushing = true;
            animator.Play("Push");
            yield return new WaitForSeconds(1f);
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                yield return null;
            }
            isPushing = false;
        }
        yield return null;
    }

    [HideInInspector] public bool isPulling = false;
    public IEnumerator PlayPull()
    {
        if (!isPulling)
        {
            isPulling = true;
            animator.Play("Pull");
            yield return new WaitForSeconds(1f);
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                yield return null;
            }
            isPulling = false;
        }
        yield return null;
    }


    #region Sounds  




    public bool WalkSoundManager()
    {
        if (_xAxisValue != 0f)
        {
            _walkEffectsHandler.PlaySound();
            return true;

        }

        return false;

    }

    #endregion
}

