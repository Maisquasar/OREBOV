using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;
using States;
using InteractObject;
using System;

public enum SoundIDs
{
    WalkInside,
    WalkOutside,
    WalkRain,
    FallInterior,
    FallExterior,
    Stress,
    Death,
    ClimbBox,
    ClimbWall,
    TransformToShadow,
    TransformToHuman,
    TransformationFail,
}

public class PlayerStatus : Entity
{
    private Dictionary<SoundIDs, SoundEffectsHandler> _soundBoard = new Dictionary<SoundIDs, SoundEffectsHandler>();

    public PlayerAction PlayerActionState;
    [HideInInspector] public Vector3 CheckpointPos;

    [SerializeField] private UIPauseMenu _pauseMenu;

    [Header("Inputs")]
    [Range(0f, 1f)]
    private float deadZone;

    [Header("Sounds")]
    [SerializeField] private GameObject _soundEffectsHandler;

    public PlayerMovement Controller;
    private ShadowCaster _caster;
    private PlayerAnimator _playerAnimator;
    private PlayerInteraction _playerInteraction;

    private Vector2 _movementDir;
    private Vector3 _previousPos;
    private Vector3 _shadowPos;

    public bool IsHide = false;
    [HideInInspector]
    private bool _isDead = false;
    private bool _isJumping = false;
    private bool _isShadow = false;
    private bool _respawn = false; // To execute repawn only once.
    private bool _exactPos = false; // To execute LerpTo only once.
    private float _stressTimer = 0.0f;
    public bool IsShadow { get { return _isShadow; } }

    public Vector2 MoveDir { get { return _movementDir; } }
    public bool Dead
    {
        get { return _isDead; }
        set
        {
            _isDead = value;
            if (value == true) PlayerDeath();
        }
    }


       #region Initiate Script 
    private void Start()
    {
        InitComponent();
        CheckpointPos = transform.position;
        foreach (SoundEffectsHandler item in _soundEffectsHandler.GetComponents<SoundEffectsHandler>())
        {
            bool found = false;
            foreach (var id in Enum.GetValues(typeof(SoundIDs)))
            {
                if (id.ToString().Equals(item.SoundName, StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        _soundBoard.Add((SoundIDs)id, item);
                        found = true;
                    }
                    catch (ArgumentException)
                    {
                        Debug.LogWarning("Duplicate element " + item.SoundName + "in sound effects");
                    }
                    break;
                }
            }
            if (!found) Debug.LogWarning("Element " + item.SoundName + " not found in sound IDs");
        }
        gameObject.GetComponent<PlayerMovement>().SetSounds(ref _soundBoard);
    }


    private void InitComponent()
    {
        Controller = gameObject.GetComponent<PlayerMovement>();
        _caster = gameObject.GetComponent<ShadowCaster>();
        _playerAnimator = gameObject.GetComponent<PlayerAnimator>();
        _playerInteraction = gameObject.GetComponent<PlayerInteraction>();
    }

    #endregion

    // Update is called once per 
    private void Update()
    {
        if (Dead)
        {
            _stressTimer = 0.0f;
            _soundBoard[SoundIDs.Stress].StopSound();
            return;
        }
        if (_stressTimer > 0) _stressTimer -= Time.deltaTime;
        else if (_soundBoard[SoundIDs.Stress].Active) _soundBoard[SoundIDs.Stress].StopSound();
        _shadowPos = _caster.GetShadowPos();
        if (_playerInteraction.Interaction != PlayerInteraction.InteractionState.Link)
        {
            Controller.Move(_movementDir.x);
            Controller.ChangeState(ref PlayerActionState);
        }
        if (_isShadow && !_playerAnimator.IsInMovement)
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
                //if (_caster.ShadowDepth < transform.position.z - 0.2f && _movementDir.y < deadZone && !_playerAnimator.IsInAmination)
                if (_caster.ShadowDepth < transform.position.z - 0.05f && !_playerAnimator.IsInAmination)
                {
                    _playerAnimator.MovePlayerPos(new Vector2(_caster.ShadowHeight, _caster.ShadowDepth), _caster.ShadowDeltaX);
                }
            }
        }
        _previousPos = transform.position;
    }

    public void OnMove(CallbackContext context)
    {
        Vector2 moveTemp = GetStickInput(context.ReadValue<Vector2>());
        //Play animation in function of pos
        _playerInteraction.AxisInput(context);
        _movementDir = moveTemp;
    }

    private Vector2 GetStickInput(Vector2 inputs)
    {
        if (Mathf.Abs(inputs.x) < deadZone) inputs.x = 0.0f;
        if (Mathf.Abs(inputs.y) < deadZone) inputs.y = 0.0f;
        return inputs;
    }

    public void PlayRightAnimation(float axis)
    {
        if (axis == 0)
            return;
        if (transform.position.x < _playerInteraction.InteractiveObjectPos.x && axis > 0 || (transform.position.x > _playerInteraction.InteractiveObjectPos.x && axis < 0))
        {
            if (!Controller.IsPulling)
                StartCoroutine(Controller.PlayPush());
        }
        else
        {
            if (!Controller.IsPushing)
                StartCoroutine(Controller.PlayPull());
        }
    }

    public void OnJump(CallbackContext context)
    {
        //if (Controller.IsTouchingWall && (PlayerActionState == PlayerAction.IDLE || PlayerActionState == PlayerAction.RUN) && _playerInteraction.Interaction != PlayerInteraction.InteractionState.Link && !_playerAnimator.IsInAmination)
        if ((PlayerActionState == PlayerAction.IDLE || PlayerActionState == PlayerAction.RUN) && _playerInteraction.Interaction != PlayerInteraction.InteractionState.Link && !_playerAnimator.IsInAmination)
            if (context.started)
                Controller.Jump();
    }

    public void OnTransformAction(CallbackContext context)
    {
        if (!context.performed || _isJumping || (PlayerActionState != PlayerAction.IDLE && PlayerActionState != PlayerAction.RUN) || _playerAnimator.IsInAmination || _playerInteraction.Interaction == PlayerInteraction.InteractionState.Link)
            return;
        if (_isShadow)
        {
            OnTransformToPlayer();
            _soundBoard[SoundIDs.TransformToHuman].PlaySound();
        }
        else
        {
            if (_caster.CanTransform(true))
            {
                OnTransformToShadow();
                _soundBoard[SoundIDs.TransformToShadow].PlaySound();
            }
            else
            {
                _soundBoard[SoundIDs.TransformationFail].PlaySound();
            }
        }
    }

    public void OnTransformToShadow()
    {
        StartCoroutine(_playerAnimator.TransformToShadowAnim());
        _isShadow = true;
        Controller.GroundType ^= LayerMask.GetMask("Shadows", "NoShadows");
        Controller.WallType ^= LayerMask.GetMask("Shadows", "NoShadows");
    }

    public void OnTransformToPlayer()
    {
        _playerAnimator.ShadowPosition = _shadowPos;
        StartCoroutine(_playerAnimator.TransformToPlayerAnim());
        _isShadow = false;
        Controller.GroundType ^= LayerMask.GetMask("Shadows", "NoShadows");
        Controller.WallType ^= LayerMask.GetMask("Shadows", "NoShadows");
    }

    public void OnInteract(CallbackContext context)
    {
        if ((PlayerActionState != PlayerAction.IDLE && PlayerActionState != PlayerAction.RUN && PlayerActionState != PlayerAction.INTERACT) || _playerInteraction.Interaction == PlayerInteraction.InteractionState.None)
            return;
        if (_isJumping || Controller.IsClimbing || !Controller.IsGrounded || _playerAnimator.IsInAmination || Controller.IsHide)
            return;

        if (_playerInteraction.Interaction == PlayerInteraction.InteractionState.Selected)
        {
            _exactPos = false;
        }


        if (_playerInteraction.ObjectType == InteractObjects.Box)
        {
            if (_playerInteraction.InteractiveObjectPos.y < transform.position.y || CheckForObstacles())
                return;

            PlayerActionState = PlayerAction.INTERACT;
            if (_exactPos)
                _playerInteraction.InteractionInput(context.started, context.canceled);
            else
                // Move Player to box.
                StartCoroutine(PlayAnimationBefore(context.started, context.canceled));
        }
        else
            _playerInteraction.InteractionInput(context.started, context.canceled);
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
            Gizmos.DrawLine(transform.position, _playerInteraction.InteractiveObjectPos - new Vector3(_playerInteraction.InteractiveObjectScale.x / 2, 0, 0) * Controller.Direction);
        }
    }

    private bool CheckForObstacles()
    {
        if (_playerInteraction.Object != null)
        {
            if (Physics.Raycast(transform.position, Vector3.right * Controller.Direction, Vector3.Distance(transform.position, _playerInteraction.InteractiveObjectPos - new Vector3(_playerInteraction.InteractiveObjectScale.x / 2, 0, 0) * Controller.Direction) - 0.2f, Controller.WallType, QueryTriggerInteraction.Ignore))
                return true;
        }
        return false;
    }

    private void Respawn()
    {
        Debug.Log("Test");
        transform.position = CheckpointPos;
        _playerAnimator.enabled = true;
        _playerInteraction.enabled = true;
        Controller.enabled = true;
        _isDead = false;
        PlayerActionState = PlayerAction.IDLE;
        Controller.SetDead(false);
        _respawn = false;
        StartCoroutine(_pauseMenu.ScreenfadeOut(1.0f, 0f));
    }

    //Set Player to the right Position
    IEnumerator PlayAnimationBefore(bool started, bool canceled)
    {
        Controller.canTurn = false;
        _exactPos = true;
        float animationDistance = 0.65f;
        float Distance = (Vector3.Distance(transform.position, _playerInteraction.InteractiveObjectPos - (Vector3.right * (_playerInteraction.InteractiveObjectScale.x / 2 + animationDistance)) * Controller.Direction));
        float Distance2 = (Vector3.Distance(transform.position, _playerInteraction.InteractiveObjectPos - (Vector3.right * (_playerInteraction.InteractiveObjectScale.x / 2 + animationDistance)) * Controller.Direction * -1));
        if (Distance > Distance2)
        {
            Controller.FlipCharacter();
            yield return StartCoroutine(LerpTo(new Vector3(_playerInteraction.InteractiveObjectPos.x - ((_playerInteraction.InteractiveObjectScale.x / 2 + animationDistance)) * Controller.Direction * -1, transform.position.y, transform.position.z), 0.1f));
        }
        else if (Distance <= Distance2)
            yield return StartCoroutine(LerpTo(new Vector3(_playerInteraction.InteractiveObjectPos.x - ((_playerInteraction.InteractiveObjectScale.x / 2 + animationDistance)) * Controller.Direction, transform.position.y, transform.position.z), 0.1f));
        _playerInteraction.InteractionInput(started, canceled);
        Controller.canTurn = true;
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

    private void PlayerDeath()
    {
        Controller.SetDead(true);
        _soundBoard[SoundIDs.Death].PlaySound();
        if (Dead && !_respawn)
            StartCoroutine(WaitBeforeRespawn());
    }

    IEnumerator WaitBeforeRespawn()
    {
        _respawn = true;
        yield return _pauseMenu.ScreenfadeIn(1.0f, 2.0f);
        Respawn();
    }


    public void Hide(bool hide)
    {
        if (hide)
        {
            StartCoroutine(Controller.PlayHide());
        }
        else
        {
            StartCoroutine(Controller.StopHide());
        }
    }
    
    public void StressPlayer(float delay)
    {
        if (delay > _stressTimer) _stressTimer = delay;
        _soundBoard[SoundIDs.Stress].PlaySound();
    }

    public void PlaySound(SoundIDs sound)
    {
        _soundBoard[sound].PlaySound();
    }
}
