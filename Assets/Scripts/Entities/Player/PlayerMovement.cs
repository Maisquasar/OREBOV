using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;
using UnityEditor;

public class PlayerMovement : EntityMovement
{
    [SerializeField] AnimationCurve velocityCurve;
    [SerializeField] float _flipTime = 0.1f;

    [Space]
    [Header("Jump Settings")]
    [Space]
    [SerializeField] float jumpHeight;
    [SerializeField] private float jumpDistance;
    private float _jumpForce;

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
    [HideInInspector] public bool IsClimbing = false;

    Vector3 LastPosBeforeFall;
    private float _lastMove;
    private readonly float margeDetectionVelocity = 0.07f;
    private float time;
    private bool _fallDefine = false;


    private new void Start()
    {
        canTurn = true;
        base.Start();
        _gravityScale = 3;
        topEdgeDetectorHeight = edgeDetectorHeight + 0.15f;
    }

    #region Draw Debug

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

    #endregion

    private void Update()
    {
        if (GetComponent<PlayerStatus>().Dead)
            return;
        //Get the pos at start fall.
        if (_rb.velocity.y < -0.1f && !_fallDefine && !_grounded)
        {
            LastPosBeforeFall = transform.position;
            _fallDefine = true;
        }
        //Check if fall damage.
        if (_grounded)
        {
            _fallDefine = false;
            if (LastPosBeforeFall != null && LastPosBeforeFall.y - transform.position.y >= GameMetric.GetGameUnit(FallDamageHeight) && !GetComponent<PlayerStatus>().Dead)
            {
                GetComponent<PlayerStatus>().Dead = true;
            }
        }

        if (!DetectWall())
            animator.SetFloat("VelocityX", _rb.velocity.x);
        else
            animator.SetFloat("VelocityX", 0);
        animator.SetFloat("VelocityY", _rb.velocity.y);
        animator.SetBool("Grounded", _grounded);

        // Can't climb if fall damage.
        if (LastPosBeforeFall.y - transform.position.y < GameMetric.GetGameUnit(FallDamageHeight))
        {
            // Edge Detection :
            RaycastHit[] topRay = Physics.RaycastAll(transform.position + new Vector3(0, topEdgeDetectorHeight, 0), Vector3.right * _direction, edgeDetectorDistance + 0.5f, GroundType, QueryTriggerInteraction.Ignore);
            RaycastHit[] downRay = Physics.RaycastAll(transform.position + new Vector3(0, edgeDetectorHeight, 0), Vector3.right * _direction, edgeDetectorDistance, GroundType, QueryTriggerInteraction.Ignore);
            foreach (var ray in downRay)
            {
                if (topRay.Length == 0 && !IsClimbing)
                {
                    StartCoroutine(PlayClimb());
                }
            }
        }
    }
    public void SetDead()
    {
        animator.SetBool("Dead", true);
        LastPosBeforeFall = GetComponent<PlayerStatus>().CheckpointPos;
        animator.SetFloat("VelocityX", 0);
    }

    public void ChangeState(ref PlayerAction State)
    {
        if (State != PlayerAction.INTERACT && State != PlayerAction.PUSHING)
        {
            if (_rb.velocity.y < -margeDetectionVelocity)
            {
                ChangeStateFunction(ref State, PlayerAction.FALL);
            }
            else if (_rb.velocity.y > margeDetectionVelocity)
                ChangeStateFunction(ref State, PlayerAction.JUMP);
            else if (_rb.velocity.x < -margeDetectionVelocity || _rb.velocity.x > margeDetectionVelocity)
                ChangeStateFunction(ref State, PlayerAction.RUN);
            else
                ChangeStateFunction(ref State, PlayerAction.IDLE);
        }
        if (State == PlayerAction.RUN)
            animator.speed = Mathf.Abs(_lastMove);
        else
            animator.speed = 1;
    }

    // Move the player.
    public override void Move(float move)
    {
        base.Move(move);
        // If climbing then can't move
        if (IsClimbing || IsPushing || IsPulling)
            return;
        _lastMove = move;

        // Set move speed.
        if (_rb.velocity.y < 0.1f)
            move *= speed;
        else if (move != 0)
            move = jumpDistance * speed * Mathf.Sign(move);

        // Ground Move
        if (_grounded)
            _rb.velocity = new Vector2(velocityCurve.Evaluate(time) * move, _rb.velocity.y);

        //Flip character
        if ((move > 0 && _direction == -1 || move < 0 && _direction == 1) && _grounded && _endOfCoroutine)
        {
            StartCoroutine(Flip(transform.rotation, transform.rotation * Quaternion.Euler(0, 180, 0), _flipTime));
        }

        time += Time.deltaTime;
    }

    public void Jump()
    {
        if (_grounded)
        {
            _jumpForce = GetJumpForce(jumpHeight);
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }



    private float GetJumpForce(float jumpHeight)
    {
        return Mathf.Sqrt(jumpHeight * -2 * (_globalGravity * _gravityScale));
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
    }

    public IEnumerator PlayClimb()
    {
        RaycastHit[] downRay = Physics.RaycastAll(transform.position + new Vector3(0, edgeDetectorHeight, 0), Vector3.right * _direction, edgeDetectorDistance, GroundType, QueryTriggerInteraction.Ignore);
        StartCoroutine(LerpTo(transform.position + Vector3.right * _direction * (downRay[0].distance - 0.4f), 0.1f));
        IsClimbing = true;
        //Play animation
        animator.Play("Climb");
        //Lock Player pos
        _rb.velocity = Vector3.zero;
        _gravityScale = 0;
        // Wait for end of animation
        yield return new WaitForSecondsRealtime(1.02f);
        // Move to animation pos.
        transform.position = transform.position + new Vector3(0.4f * _direction, 1.7f, 0);
        // Reset Grabity scale.
        _gravityScale = 3;
        // Transition to Idle.
        yield return new WaitForSeconds(0.75f);
        IsClimbing = false;
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

    [HideInInspector] public bool IsPushing = false;
    // Play push animation.
    public IEnumerator PlayPush()
    {
        if (!IsPushing)
        {
            IsPushing = true;
            animator.Play("Push");
            yield return new WaitForSeconds(1f);
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                yield return null;
            }
            IsPushing = false;
        }
        yield return null;
    }

    [HideInInspector] public bool IsPulling = false;
    // Play pull animation.
    public IEnumerator PlayPull()
    {
        if (!IsPulling)
        {
            IsPulling = true;
            animator.Play("Pull");
            yield return new WaitForSeconds(1f);
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                yield return null;
            }
            IsPulling = false;
        }
        yield return null;
    }

}

