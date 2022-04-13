using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileEnemyMovement : EntityMovement
{
    [SerializeField] AnimationCurve _velocityCurve;

    Vector3 GoTo;
    float time;

    private new void Start()
    {
        base.Start();
        _direction = -1;
        _rayGroundSize = 2;
        _rayWallSize = 1;
    }

    new private void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GoTo, 1);
    }

    public void Move(float move)
    {
        if (GoTo == null)
            return;
        move *= speed;
        if (GoTo.x > transform.position.x)
            _direction = 1;
        else if (GoTo.x < transform.position.x)
            _direction = -1;

        if (_grounded)
            _rb.velocity = new Vector2(_velocityCurve.Evaluate(time) * move, _rb.velocity.y);

        if ((move > 0 && _direction == -1 || move < 0 && _direction == 1) && _grounded && _endOfCoroutine)
        {
            StartCoroutine(Flip(transform.rotation, transform.rotation * Quaternion.Euler(0, 180, 0), 0.1f));
        }
        time += Time.deltaTime;
    }

    public void NewCheckpoint(Vector3 to)
    {
        GoTo = to;
    }

    override protected void WallDetection()
    {
        for (int i = 0; i < ray.Count; i++)
        {
            Vector3 WallPos = transform.position + new Vector3(0, ray[i], 0);
            RaycastHit[] tmpHit = Physics.RaycastAll(WallPos, Vector3.right * _direction, _rayWallSize, GroundType, QueryTriggerInteraction.Ignore);
            for (int j = 0; j < tmpHit.Length; j++)
            {
                if (!tmpHit[i].transform.GetComponent<PlayerStatus>())
                {
                    Vector3 tmp = _rb.velocity;
                    tmp.x = 0;
                    _rb.velocity = tmp;
                }
            }
        }
    }

    protected override bool DetectWall()
    {
        for (int i = 0; i < ray.Count; i++)
        {
            Vector3 WallPos = transform.position + new Vector3(0, ray[i], 0);
            RaycastHit[] tmp = Physics.RaycastAll(WallPos, Vector3.right * _direction, _rayWallSize, GroundType, QueryTriggerInteraction.Ignore);
            for (int j = 0; j < tmp.Length; j++)
            {
                if (!tmp[i].transform.GetComponent<PlayerStatus>())
                    return true;
            }
        }
        return false;
    }

}
