using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : Enemy
{
    [SerializeField] public StaticEnemyMovement _controller;


    public override EntityMovement Controller { get { return _controller; } }

    public override void Shoot()
    {
        _controller.animator.SetBool("Shooting", true);
    }
}
