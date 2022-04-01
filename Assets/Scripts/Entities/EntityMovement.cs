using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityMovement : MonoBehaviour
{
    [SerializeField] protected LayerMask GroundType;
    protected float rayGroundSize;
    protected float rayCeilingSize;
    protected float rayWallSize;

    protected float globalGravity = -9.81f;
    [SerializeField] protected float gravityScale = 1;

    protected Rigidbody rb;
    protected bool grounded;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rayGroundSize = 1f;
        rayCeilingSize = 1f;
        rayWallSize = 0.5f;
    }

    private void OnDrawGizmos()
    {
        //Draw Debug line for ground
        Gizmos.color = Color.blue;
        Vector3 groundPos = transform.position + new Vector3(0, 0, 0);
        Gizmos.DrawLine(groundPos, groundPos + Vector3.down * rayGroundSize);

        //Draw Debug line for Wall
        Gizmos.color = Color.blue;
        for (int i = 0; i < 3; i++)
        {
            Vector3 WallPos = transform.position + new Vector3(0, -transform.localScale.y / 3 + transform.localScale.y / 3 * i, 0);
            Gizmos.DrawLine(WallPos, WallPos + Vector3.left * rayWallSize);

            Gizmos.DrawLine(WallPos, WallPos + Vector3.right * rayWallSize);
        }
    }

    virtual protected void FixedUpdate()
    {
        // Ground Detection
        grounded = false;
        if (Physics.Raycast(transform.position, Vector3.down, rayGroundSize, GroundType))
        {
            grounded = true;
        }

        // Ceiling Detection
        if (Physics.Raycast(transform.position, Vector3.up, rayCeilingSize, GroundType))
        {
            rb.velocity = new Vector3(rb.velocity.x, -2f);
        }

        // Wall Detection
        for (int i = 0; i < 3; i++)
        {
            Vector3 WallPos = transform.position + new Vector3(0, -transform.localScale.y / 3 + transform.localScale.y / 3 * i, 0);
            if (Physics.Raycast(WallPos, Vector3.left, rayWallSize, GroundType) && rb.velocity.x < -0.1f || Physics.Raycast(WallPos, Vector3.right, rayWallSize, GroundType) && rb.velocity.x > 0.1f)
            {
                rb.velocity = new Vector3(0, rb.velocity.y);
            }
        }


        // Set Gravity.
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }


    public virtual void Move() { }
}
