using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : Enemy
{
    [SerializeField] public StaticEnemyMovement _controller;
    public override EntityMovement Controller { get { return _controller; } }

    public override void Start()
    {
        _entityController = _controller;
        base.Start();
    }

    public override void Shoot()
    {
        _controller.animator.SetBool("Shooting", true);
    }

    protected override void EndShooting()
    {
        base.EndShooting();
        _controller.animator.SetBool("Shooting", false);

    }
}
