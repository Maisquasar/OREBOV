using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityMovement : AmbientTypeHolder
{
    [SerializeField] public Animator animator;

    [Space]
    [Header("Collision Settings")]
    [Space]
    [SerializeField] public LayerMask GroundType;
    [SerializeField] public LayerMask WallType;
    [Tooltip("Manually place rays (May lag if too much)")]
    [SerializeField] protected List<float> _wallRays;
    [SerializeField] protected List<float> _groundRays;

    [Space]
    [Header("Velocity Settings")]
    [Space]
    [SerializeField] protected float speed;

    protected SoundEffectsHandler _walkInsideEffectsHandler;
    protected SoundEffectsHandler _walkOutsideEffectsHandler;
    protected SoundEffectsHandler _walkRainEffectsHandler;
    protected SoundEffectsHandler _landInsideEffectHandler;
    protected SoundEffectsHandler _landOutsideEffectHandler;

    [HideInInspector] public float _globalGravity = -9.81f;
    [HideInInspector] public float _gravityScale = 1;

    protected float _rayGroundSize = 1.1f;
    protected float _rayCeilingSize = 1.1f;
    protected float _rayWallSize = 0.31f;
    protected float _direction = 1;
    protected float _xAxisValue;

    public float Direction { get { return _direction; } set { _direction = value; } }

    protected bool _touchWall = false;
    public bool IsTouchingWall { get { return _touchWall; } }

    protected Rigidbody _rb;
    protected bool _grounded;
    protected bool _endOfCoroutine = true;

    public bool IsGrounded { get { return _grounded; } }

    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public void SetSounds(ref Dictionary<SoundIDs, SoundEffectsHandler> sounds)
    {
        _walkInsideEffectsHandler = sounds[SoundIDs.WalkInside];
        _walkOutsideEffectsHandler = sounds[SoundIDs.WalkOutside];
        _walkRainEffectsHandler = sounds[SoundIDs.WalkRain];
        _landInsideEffectHandler = sounds[SoundIDs.FallInterior];
        _landOutsideEffectHandler = sounds[SoundIDs.FallExterior];
    }

    #region Draw Debug

    protected void OnDrawGizmos()
    {
        //Draw Debug line for ground
        Gizmos.color = Color.blue;

        for (int i = 0; i < _groundRays.Count; i++)
        {
            Vector3 WallPos = transform.position + new Vector3(_groundRays[i], 0, 0);
            Gizmos.DrawRay(WallPos, Vector3.down * _rayGroundSize);
        }

        //Draw Debug line for ceiling
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector3.up * _rayCeilingSize);

        //Draw Debug line for Wall
        Gizmos.color = Color.blue;
        for (int i = 0; i < _wallRays.Count; i++)
        {
            Vector3 WallPos = transform.position + new Vector3(0, _wallRays[i], 0);
            Gizmos.DrawRay(WallPos, Vector3.left * _rayWallSize);
            Gizmos.DrawRay(WallPos, Vector3.right * _rayWallSize);
        }
    }

    #endregion

    virtual protected void FixedUpdate()
    {
        // Ground Detection
        GroundDetection();

        // Ceiling Detection
        CeilingDetection();

        // Wall Detection
        WallDetection();
        SetGravity();
    }

    protected void GroundDetection()
    {

        for (int i = 0; i < _groundRays.Count; i++)
        {
            Vector3 GroundPos = transform.position + new Vector3(_groundRays[i], 0, 0);
            if (Physics.Raycast(GroundPos, Vector3.down, _rayGroundSize, GroundType, QueryTriggerInteraction.Ignore))
            {
                if (!_grounded)
                {
                    LandOnGround();
                }
                break;
            }
            if (i == 2)
            {
                _grounded = false;
            }
        }
    }

    virtual protected void WallDetection()
    {
        _touchWall = false;
        if (DetectWall())
        {
            Vector3 tmp = _rb.velocity;
            tmp.x = 0;
            _rb.velocity = tmp;
            _touchWall = true;
        }
    }

    protected void CeilingDetection()
    {

        if (Physics.Raycast(transform.position, Vector3.up, _rayCeilingSize, GroundType, QueryTriggerInteraction.Ignore))
        {
            _rb.velocity = new Vector3(_rb.velocity.x, -2f);
        }
    }

    protected void SetGravity()
    {
        Vector3 gravity = _globalGravity * _gravityScale * Vector3.up;
        _rb.AddForce(gravity, ForceMode.Acceleration);
    }

    protected virtual void LandOnGround()
    {
        _grounded = true;
        if (AmbientType == AmbientSoundType.Interior)
        {
            _landInsideEffectHandler.PlaySound();
        }
        else
        {
            _landOutsideEffectHandler.PlaySound();
        }
    }

    protected virtual bool DetectWall()
    {
        for (int i = 0; i < _wallRays.Count; i++)
        {
            // Set all Ray pos
            Vector3 WallPos = transform.position + new Vector3(0, _wallRays[i], 0);
            if ((Physics.Raycast(WallPos, Vector3.left, _rayWallSize, WallType, QueryTriggerInteraction.Ignore) && _rb.velocity.x < -0.1f) || (Physics.Raycast(WallPos, Vector3.right, _rayWallSize, WallType, QueryTriggerInteraction.Ignore) && _rb.velocity.x > 0.1f))
            {
                return true;
            }
        }
        return false;
    }

    [HideInInspector] public bool canTurn = true;
    virtual protected IEnumerator Flip(Quaternion initial, Quaternion goTo, float duration)
    {
        if (!canTurn)
            yield break;
        _endOfCoroutine = false;
        _direction *= -1;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        transform.rotation = goTo;
        _endOfCoroutine = true;
    }

    public virtual void Move(float move)
    {
        _xAxisValue = move;
    }

    #region Sounds  
    public bool WalkSoundManager()
    {
        if (_xAxisValue != 0f)
        {
            switch (AmbientType)
            {
                case AmbientSoundType.Exterior:
                    _walkOutsideEffectsHandler.PlaySound();
                    break;
                case AmbientSoundType.Rain:
                    _walkRainEffectsHandler.PlaySound();
                    break;
                default:
                    _walkInsideEffectsHandler.PlaySound();
                    break;
            }
            return true;
        }
        return false;
    }

    #endregion
}
