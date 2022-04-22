using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileEnemyMovement : EntityMovement
{
    [SerializeField] AnimationCurve _velocityCurve;

    public Vector3 GoTo { get { return _goTo; } }

    Vector3 _goTo;
    float _time;

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
        Gizmos.DrawSphere(_goTo, 1);
    }

    private void Update()
    {
        if (!DetectWall())
            animator.SetFloat("VelocityX", _rb.velocity.x);
        else
            animator.SetFloat("VelocityX", 0);
    }

    public override void Move(float move)
    {
        base.Move(move);
        if (_goTo == Vector3.zero || _goTo == null)
            return;
        move *= speed;
        if ((_goTo.x > transform.position.x && Mathf.Sign(move) == -1) || (_goTo.x < transform.position.x && Mathf.Sign(move) == 1))
            move *= -1;

        if (_grounded)
            _rb.velocity = new Vector2(_velocityCurve.Evaluate(_time) * move, _rb.velocity.y);

        if (_grounded && _endOfCoroutine)
        {
            if (move < 0 && _direction == 1 || move < 0 && transform.rotation != Quaternion.Euler(0, 0, 0))
                StartCoroutine(Flip(transform.rotation, Quaternion.Euler(0, 0, 0), 0.1f));
            else if (move > 0 && _direction == -1 || move > 0 && transform.rotation != Quaternion.Euler(0, 180, 0))
                StartCoroutine(Flip(transform.rotation, Quaternion.Euler(0, 180, 0), 0.1f));
        }
        _time += Time.deltaTime;
    }


    public void NewCheckpoint(Vector3 to)
    {
        Debug.Log("New Checkpoint");
        _goTo = to;
    }


    override protected void WallDetection()
    {
        for (int i = 0; i < _wallRays.Count; i++)
        {
            Vector3 WallPos = transform.position + new Vector3(0, _wallRays[i], 0);
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
        for (int i = 0; i < _wallRays.Count; i++)
        {
            Vector3 WallPos = transform.position + new Vector3(0, _wallRays[i], 0);
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
