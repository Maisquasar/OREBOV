using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : EntityMovement
{
    [SerializeField] private float jumpForce;
    [SerializeField] private bool airControl;
    [SerializeField] AnimationCurve velocityCurve;
    float time;
    private Vector3 velocity = Vector3.zero;

    public void Move(float move, bool jump)
    {
        if (grounded || airControl)
        {
            rb.velocity = new Vector2(velocityCurve.Evaluate(time) * move, rb.velocity.y);
        }
        if (grounded && jump)
        {
            grounded = false;
            rb.AddForce((Vector3.up * (jumpForce) * 10));
        }
        time += Time.deltaTime;
    }

}
