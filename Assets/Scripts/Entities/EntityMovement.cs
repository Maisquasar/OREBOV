using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityMovement : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [Space]
    [Header("Collision Settings")]
    [Space]
    [SerializeField] public LayerMask GroundType;
    [Tooltip("Manually place rays (May lag if too much)")]
    [SerializeField] private List<float> ray;
    [Space]
    [Header("Velocity Settings")]
    [Space]
    [SerializeField] protected float speed;

    protected float rayGroundSize = 1.1f;
    protected float rayCeilingSize = 1.1f;
    protected float rayWallSize = 0.31f;
    protected float direction = 1;
    public float Direction { get { return direction; } }

    protected float globalGravity = -9.81f;
    protected float gravityScale = 1;
    float offset = 0.18f;

    protected Rigidbody rb;
    protected bool grounded;
    protected bool endOfCoroutine = true;

    public bool IsGrounded { get { return grounded; } }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void OnDrawGizmos()
    {
        //Draw Debug line for ground
        Gizmos.color = Color.blue;
        for (int i = 0; i < 3; i++)
            Gizmos.DrawRay(new Vector3(transform.position.x - offset + offset * i, transform.position.y, transform.position.z), new Vector3(0, -rayGroundSize, 0));

        //Draw Debug line for ceiling
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector3.up * rayCeilingSize);

        //Draw Debug line for Wall
        Gizmos.color = Color.blue;
        for (int i = 0; i < ray.Count; i++)
        {
            Vector3 WallPos = transform.position + new Vector3(0, ray[i], 0);
            Gizmos.DrawRay(WallPos, Vector3.left * rayWallSize);
            Gizmos.DrawRay(WallPos, Vector3.right * rayWallSize);
        }
    }

    virtual protected void FixedUpdate()
    {
        // Ground Detection
        for (int i = 0; i < 3; i++)
        {
            if (Physics.Raycast(new Vector3(transform.position.x - offset + offset * i, transform.position.y, transform.position.z), Vector3.down, rayGroundSize, GroundType, QueryTriggerInteraction.Ignore))
            {
                if (!grounded)
                {
                    LandOnGround();
                }
                break;
            }
            if (i == 2)
            {
                grounded = false;
            }
        }

        // Ceiling Detection
        if (Physics.Raycast(transform.position, Vector3.up, rayCeilingSize, GroundType, QueryTriggerInteraction.Ignore))
        {
            rb.velocity = new Vector3(rb.velocity.x, -2f);
        }

        // Wall Detection
        for (int i = 0; i < ray.Count; i++)
        {
            // Set all Ray pos
            Vector3 WallPos = transform.position + new Vector3(0, ray[i], 0);
            if ((Physics.Raycast(WallPos, Vector3.left, rayWallSize, GroundType, QueryTriggerInteraction.Ignore) && rb.velocity.x < -0.1f) || (Physics.Raycast(WallPos, Vector3.right, rayWallSize, GroundType, QueryTriggerInteraction.Ignore) && rb.velocity.x > 0.1f))
            {
                Vector3 tmp = rb.velocity;
                tmp.x = 0;
                rb.velocity = tmp;
            }
        }

        // Set Gravity.
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }



    protected virtual void LandOnGround()
    {
        grounded = true;
    }

    protected virtual bool DetectWall()
    {
        for (int i = 0; i < ray.Count; i++)
        {
            // Set all Ray pos
            Vector3 WallPos = transform.position + new Vector3(0, ray[i], 0);
            if ((Physics.Raycast(WallPos, Vector3.left, rayWallSize, GroundType, QueryTriggerInteraction.Ignore) && rb.velocity.x < -0.1f) || (Physics.Raycast(WallPos, Vector3.right, rayWallSize, GroundType, QueryTriggerInteraction.Ignore) && rb.velocity.x > 0.1f))
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
        endOfCoroutine = false;
        direction *= -1;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        transform.rotation = goTo;
        endOfCoroutine = true;
    }

    public virtual void Move() { }
}
