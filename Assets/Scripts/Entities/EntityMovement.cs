using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityMovement : MonoBehaviour
{
    [SerializeField] public LayerMask GroundType;
    [Tooltip("Manually place rays (May lag if too much)")]
    [SerializeField] List<float> ray;

    protected float rayGroundSize = 1.1f;
    protected float rayCeilingSize = 1.0f;
    protected float rayWallSize = 0.51f;

    protected float globalGravity = -9.81f;
    [SerializeField] protected float gravityScale = 1;

    protected Rigidbody rb;
    protected bool grounded;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void OnDrawGizmos()
    {
        //Draw Debug line for ground
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector3.down * rayGroundSize);

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
        grounded = false;
        if (Physics.Raycast(transform.position, Vector3.down, rayGroundSize, GroundType, QueryTriggerInteraction.Ignore))
        {
            grounded = true;
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


    public virtual void Move() { }
}
