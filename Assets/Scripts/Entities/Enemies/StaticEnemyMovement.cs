using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemyMovement : EntityMovement
{
    [SerializeField] AnimationCurve velocityCurve;
    float time;

    public void Move(float move)
    {
        move *= speed;
        if (grounded)
            rb.velocity = new Vector2(velocityCurve.Evaluate(time) * move, rb.velocity.y);

        if (DetectWall())
            direction *= -1;
        time += Time.deltaTime;
    }

    private new void Start()
    {
        base.Start();
        direction = -1;
    }
}
