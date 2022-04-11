    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityMovement : MonoBehaviour
{
    [SerializeField] public Animator animator;

    [Space]    [Header("Collision Settings")]    [Space]
    [SerializeField] public LayerMask GroundType;
    [Tooltip("Manually place rays (May lag if too much)")]
    [SerializeField] private List<float> ray;

    [Space]    [Header("Velocity Settings")]    [Space]
    [SerializeField] protected float speed;

    protected float _rayGroundSize = 1.1f;
    protected float _rayCeilingSize = 1.1f;
    protected float _rayWallSize = 0.31f;
    protected float _direction = 1;
    public float Direction { get { return _direction; } }

    protected float _globalGravity = -9.81f;
    protected float _gravityScale = 1;
    protected float _offset = 0.18f;

    protected Rigidbody _rb;
    protected bool _grounded;
    protected bool _endOfCoroutine = true;

    public bool IsGrounded { get { return _grounded; } }

    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    #region Draw Debug

    protected void OnDrawGizmos()
    {
        //Draw Debug line for ground
        Gizmos.color = Color.blue;
        for (int i = 0; i < 3; i++)
            Gizmos.DrawRay(new Vector3(transform.position.x - _offset + _offset * i, transform.position.y, transform.position.z), new Vector3(0, -_rayGroundSize, 0));

        //Draw Debug line for ceiling
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector3.up * _rayCeilingSize);

        //Draw Debug line for Wall
        Gizmos.color = Color.blue;
        for (int i = 0; i < ray.Count; i++)
        {
            Vector3 WallPos = transform.position + new Vector3(0, ray[i], 0);
            Gizmos.DrawRay(WallPos, Vector3.left * _rayWallSize);
            Gizmos.DrawRay(WallPos, Vector3.right * _rayWallSize);
        }
    }

    #endregion

    virtual protected void FixedUpdate()
    {
        // Ground Detection
        GroundDetection();

        // Ceiling Detection
        CeilingDetection();

        // Wall Detection
        //if (!GetComponent<PlayerStatus>())
        WallDetection();
        SetGravity();
    }

    protected void GroundDetection()
    {

        for (int i = 0; i < 3; i++)
        {
            if (Physics.Raycast(new Vector3(transform.position.x - _offset + _offset * i, transform.position.y, transform.position.z), Vector3.down, _rayGroundSize, GroundType, QueryTriggerInteraction.Ignore))
            {
                if (!_grounded)
                {
                    LandOnGround();
                }
                break;
            }
            if (i == 2)
            {
                _grounded = false;
            }
        }
    }

    protected void WallDetection()
    {
        for (int i = 0; i < ray.Count; i++)
        {
            // Set all Ray pos
            Vector3 WallPos = transform.position + new Vector3(0, ray[i], 0);
            if ((Physics.Raycast(WallPos, Vector3.left, _rayWallSize, GroundType, QueryTriggerInteraction.Ignore) && _rb.velocity.x < -0.1f) || (Physics.Raycast(WallPos, Vector3.right, _rayWallSize, GroundType, QueryTriggerInteraction.Ignore) && _rb.velocity.x > 0.1f))
            {
                Vector3 tmp = _rb.velocity;
                tmp.x = 0;
                _rb.velocity = tmp;
            }
        }
    }

    protected void CeilingDetection()
    {

        if (Physics.Raycast(transform.position, Vector3.up, _rayCeilingSize, GroundType, QueryTriggerInteraction.Ignore))
        {
            _rb.velocity = new Vector3(_rb.velocity.x, -2f);
        }
    }

    protected void SetGravity()
    {
        Vector3 gravity = _globalGravity * _gravityScale * Vector3.up;
        _rb.AddForce(gravity, ForceMode.Acceleration);
    }

    protected virtual void LandOnGround()
    { 
        _grounded = true;   
    }

    protected virtual bool DetectWall()
    {
        for (int i = 0; i < ray.Count; i++)
        {
            // Set all Ray pos
            Vector3 WallPos = transform.position + new Vector3(0, ray[i], 0);
            if ((Physics.Raycast(WallPos, Vector3.left, _rayWallSize, GroundType, QueryTriggerInteraction.Ignore) && _rb.velocity.x < -0.1f) || (Physics.Raycast(WallPos, Vector3.right, _rayWallSize, GroundType, QueryTriggerInteraction.Ignore) && _rb.velocity.x > 0.1f))
            {
                return true;
            }
        }
        return false;
    }

    [HideInInspector] public bool canTurn = true;
    protected IEnumerator Flip(Quaternion initial, Quaternion goTo, float duration)
    {
        if (!canTurn)
            yield break;
        _endOfCoroutine = false;
        _direction *= -1;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        transform.rotation = goTo;
        _endOfCoroutine = true;
    }

    public virtual void Move() { }
}
