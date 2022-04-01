using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityMovement : MonoBehaviour
{
    [SerializeField] protected Transform groundCheck;
    protected float groundCheckRadius = 0.1f;
    [SerializeField] protected Transform ceilingCheck;
    protected float ceilingCheckRadius = 0.4f;
    [SerializeField] protected LayerMask GroundType;

    protected float globalGravity = -9.81f;
    [SerializeField] protected float gravityScale = 1;

    protected Rigidbody rb;
    protected bool grounded;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }

    virtual protected void FixedUpdate()
    {
        grounded = false;

        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, GroundType);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
            }
        }
        // Set Gravity.
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }


    public virtual void Move() { }
}
