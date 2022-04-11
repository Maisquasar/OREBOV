using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;
using States;
using InteractObject;


public class PlayerStatus : Entity
{
    [HideInInspector] public PlayerAction PlayerActionState;
    [HideInInspector] public Vector3 CheckpointPos;

    [SerializeField] private UIPauseMenu _pauseMenu;

    [Header("Sounds")]
    [SerializeField] private SoundEffectsHandler _shadowEffectHandler;

    private PlayerMovement _controller;
    private ShadowCaster _caster;
    private PlayerAnimator _playerAnimator;
    private PlayerInteraction _playerInteraction;

    private Vector2 _movementDir;
    private Vector3 _previousPos;

    private bool _isDead = false;
    private bool _isJumping = false;
    private bool _isShadow = false;
    private bool _respawn = false; // To execute repawn only once.
    private bool _exactPos = false; // To execute LerpTo only once.

    public Vector2 MoveDir { get { return _movementDir; } }
    public bool Dead { get { return _isDead; } set { _isDead = value; } }
    public bool IsShadow { get { return _isShadow; } }


    #region Initiate Script
    private void Start()
    {
        InitComponent();
        CheckpointPos = transform.position;
    }


    private void InitComponent()
    {
        _controller = gameObject.GetComponent<PlayerMovement>();
        _caster = gameObject.GetComponent<ShadowCaster>();
        _playerAnimator = gameObject.GetComponent<PlayerAnimator>();
        _playerInteraction = gameObject.GetComponent<PlayerInteraction>();
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Dead && !_respawn)
            StartCoroutine(WaitBeforeRespawn());
        else if (Dead)
            return;

        if (!_playerAnimator.IsInAmination && _playerInteraction.Interaction != PlayerInteraction.InteractionState.Link)
        {
            _controller.Move(_movementDir.x, _isJumping);
            _controller.ChangeState(ref PlayerActionState);
        }
        if (_isJumping)
            _isJumping = false;
        if (_isShadow)
        {
            if (!_caster.CanTransform(false))
            {
                if (_caster.DoesCurrentLightEject)
                {
                    OnTransformToPlayer();
                }
                else
                {
                    Vector3 pos = transform.position;
                    transform.position = new Vector3(_previousPos.x, transform.position.y, _previousPos.z);
                    if (!_caster.CanTransform(false))
                    {
                        transform.position = pos;
                        OnTransformToPlayer();
                    }
                }
            }
            else
            {
                if (_caster.ShadowDepth < transform.position.z - 0.2f && !_playerAnimator.IsInMovement && !_playerAnimator.IsInAmination)
                {
                    _playerAnimator.MovePlayerDepthTo(new Vector2(_caster.ShadowHeight, _caster.ShadowDepth));
                }
            }
        }
        _previousPos = transform.position;
    }

    public void OnMove(CallbackContext context)
    {
        Vector3 moveTemp = context.ReadValue<Vector2>();
        if (Mathf.Abs(moveTemp.x) < 0.03f) moveTemp.x = 0.0f;
        if (Mathf.Abs(moveTemp.y) < 0.03f) moveTemp.y = 0.0f;
        //Play animation in function of pos
        if (_playerInteraction.Interaction == PlayerInteraction.InteractionState.Link)
        {
            if (moveTemp.normalized.x == _movementDir.normalized.x)
            {
                _movementDir = moveTemp;
            }
            _playerInteraction.AxisInput(context);
        }
        _movementDir = moveTemp;
    }

    public void PlayRightAnimation(float axis)
    {
        if (axis == 0)
            return;
        if (transform.position.x < _playerInteraction.InteractiveObjectPos.x && axis > 0 || (transform.position.x > _playerInteraction.InteractiveObjectPos.x && axis < 0))
        {
            if (!_controller.IsPulling)
                StartCoroutine(_controller.PlayPush());
        }
        else
        {
            if (!_controller.IsPushing)
                StartCoroutine(_controller.PlayPull());
        }
    }

    public void OnJump(CallbackContext context)
    {
        if ((PlayerActionState == PlayerAction.IDLE || PlayerActionState == PlayerAction.RUN) && _playerInteraction.Interaction != PlayerInteraction.InteractionState.Link)
            if (context.performed)
                _isJumping = true;
    }

    public void OnTransformAction(CallbackContext context)
    {
        if (!context.performed || _isJumping || (PlayerActionState != PlayerAction.IDLE && PlayerActionState != PlayerAction.RUN) || _playerAnimator.IsInAmination || _playerInteraction.Interaction == PlayerInteraction.InteractionState.Link)
            return;
        if (_isShadow)
        {
            OnTransformToPlayer();
        }
        else
        {
            if (_caster.CanTransform(true))
            {
                OnTransformToShadow();
            }
        }
        _shadowEffectHandler.PlaySound();
    }

    public void OnTransformToShadow()
    {
        StartCoroutine(_playerAnimator.TransformToShadowAnim());
        _isShadow = true;
        _controller.GroundType ^= LayerMask.GetMask("Shadows", "NoShadows");
    }

    public void OnTransformToPlayer()
    {
        _playerAnimator.ShadowPosition = _caster.GetShadowPos();
        StartCoroutine(_playerAnimator.TransformToPlayerAnim());
        _isShadow = false;
        _controller.GroundType ^= LayerMask.GetMask("Shadows", "NoShadows");
    }

    public void OnInteract(CallbackContext context)
    {
        if ((PlayerActionState != PlayerAction.IDLE && PlayerActionState != PlayerAction.RUN && PlayerActionState != PlayerAction.INTERACT) || _playerInteraction.Interaction == PlayerInteraction.InteractionState.None)
            return;
        if (_isJumping || _controller.IsClimbing || !_controller.IsGrounded || _playerAnimator.IsInAmination)
            return;
        if (_playerInteraction.InteractiveObjectPos.y + 0.25f < transform.position.y || CheckForObstacles())
            return;

        PlayerActionState = PlayerAction.INTERACT;
        if (_playerInteraction.ObjectType == InteractObjects.Box)
        {
            if (_exactPos)
                _playerInteraction.InteractionInput(context.started, context.canceled);
            else
                // Move Player to box.
                StartCoroutine(PlayAnimationBefore(context.started, context.canceled));
        }
        else
            _playerInteraction.InteractionInput(context.started, context.canceled);

        if (_playerInteraction.Interaction == PlayerInteraction.InteractionState.Selected)
        {
            PlayerActionState = PlayerAction.IDLE;
            _exactPos = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (_playerInteraction == null) _playerInteraction = gameObject.GetComponent<PlayerInteraction>();
        if (CheckForObstacles())
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        if (_playerInteraction.Interaction == PlayerInteraction.InteractionState.Selected)
        {
            Gizmos.DrawLine(transform.position, _playerInteraction.InteractiveObjectPos - new Vector3(_playerInteraction.InteractiveObjectScale.x / 2, 0, 0) * _controller.Direction);
        }
    }

    private bool CheckForObstacles()
    {
        if (_playerInteraction.Object != null)
        {
            if (Physics.Raycast(transform.position, Vector3.right * _controller.Direction, Vector3.Distance(transform.position, _playerInteraction.InteractiveObjectPos - new Vector3(_playerInteraction.InteractiveObjectScale.x / 2, 0, 0) * _controller.Direction) - 0.1f, _controller.GroundType, QueryTriggerInteraction.Ignore))
                return true;
        }
        return false;
    }

    private void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Set Player to the right Position
    IEnumerator PlayAnimationBefore(bool started, bool canceled)
    {
        _controller.canTurn = false;
        _exactPos = true;
        float animationDistance = 0.65f;
        float Distance = (Vector3.Distance(transform.position, _playerInteraction.InteractiveObjectPos - (Vector3.right * (_playerInteraction.InteractiveObjectScale.x / 2 + animationDistance)) * _controller.Direction));
        float Distance2 = (Vector3.Distance(transform.position, _playerInteraction.InteractiveObjectPos - (Vector3.right * (_playerInteraction.InteractiveObjectScale.x / 2 + animationDistance)) * _controller.Direction * -1));
        if (Distance > Distance2)
        {
            _controller.FlipCharacter();
            yield return StartCoroutine(LerpTo(new Vector3(_playerInteraction.InteractiveObjectPos.x - ((_playerInteraction.InteractiveObjectScale.x / 2 + animationDistance)) * _controller.Direction * -1, transform.position.y, transform.position.z), 0.1f));
        }
        else if (Distance <= Distance2)
            yield return StartCoroutine(LerpTo(new Vector3(_playerInteraction.InteractiveObjectPos.x - ((_playerInteraction.InteractiveObjectScale.x / 2 + animationDistance)) * _controller.Direction, transform.position.y, transform.position.z), 0.1f));
        _playerInteraction.InteractionInput(started, canceled);
        _controller.canTurn = true;
    }

    //Change player position with duration time.
    IEnumerator LerpTo(Vector3 goTo, float duration)
    {
        Vector3 initial = transform.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(initial, goTo, t / duration);
            yield return 0;
        }
        transform.position = goTo;
    }

    IEnumerator WaitBeforeRespawn()
    {
        _respawn = true;
        yield return _pauseMenu.ScreenfadeIn(1.0f, 2.0f);
        Respawn();
    }
}
