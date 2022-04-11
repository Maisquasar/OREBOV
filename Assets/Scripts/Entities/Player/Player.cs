using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;
using States;


public class Player : Entity
{
    public PlayerMovement Controller;
    [HideInInspector] public PlayerAction PlayerActionState;
    private ShadowCaster Caster;
    private PlayerAnimator PlayerAnimator;
    private PlayerInteraction PlayerInteraction;

    [SerializeField]
    private UIPauseMenu _PauseMenu;

    public Vector3 CheckpointPos;

    float _shadowTime;
    bool _isDead = false;

    public bool Dead { get { return _isDead; } set { _isDead = value; } }
    bool _isJumping = false;
    public bool IsShadow { get { return _isShadow; } }
    bool _isShadow = false;
    Vector2 movementDir;
    Vector3 previousPos;
    public Vector2 MoveDir { get { return movementDir; } }

    [Header("Sounds")]
    [SerializeField]
    private SoundEffectsHandler _shadowEffectHandler;

    private void Start()
    {
        Controller = gameObject.GetComponent<PlayerMovement>();
        Caster = gameObject.GetComponent<ShadowCaster>();
        PlayerAnimator = gameObject.GetComponent<PlayerAnimator>();
        PlayerInteraction = gameObject.GetComponent<PlayerInteraction>();
        CheckpointPos = transform.position;
    }
    // Update is called once per frame
    bool respawn = false;
    void Update()
    {
        if (Dead && !respawn)
            StartCoroutine(WaitBeforeRespawn());
        else if (Dead)
            return;

        if (!PlayerAnimator.IsInAmination && PlayerInteraction.Interaction != PlayerInteraction.InteractionState.Link)
        {
            Controller.Move(movementDir.x, _isJumping);
            Controller.ChangeState(ref PlayerActionState);
        }
        if (_isJumping)
            _isJumping = false;
        if (_isShadow)
        {
            if (!Caster.CanTransform(false))
            {
                if (Caster.DoesCurrentLightEject)
                {
                    OnTransformToPlayer();
                }
                else
                {
                    Vector3 pos = transform.position;
                    transform.position = new Vector3(previousPos.x, transform.position.y, previousPos.z);
                    if (!Caster.CanTransform(false))
                    {
                        transform.position = pos;
                        OnTransformToPlayer();
                    }
                }
            }
            else
            {
                if (Caster.ShadowDepth < transform.position.z - 0.2f && !PlayerAnimator.IsInMovement && !PlayerAnimator.IsInAmination)
                {
                    PlayerAnimator.MovePlayerDepthTo(new Vector2(Caster.ShadowHeight, Caster.ShadowDepth));
                }
            }
        }
        previousPos = transform.position;
    }

    float lastMovementDir;
    public void OnMove(CallbackContext context)
    {
        Vector3 moveTemp = context.ReadValue<Vector2>();
        if (Mathf.Abs(moveTemp.x) < 0.03f) moveTemp.x = 0.0f;
        if (Mathf.Abs(moveTemp.y) < 0.03f) moveTemp.y = 0.0f;
        //Play animation in function of pos
        if (PlayerInteraction.Interaction == PlayerInteraction.InteractionState.Link)
        {
            if (moveTemp.normalized.x == movementDir.normalized.x)
            {
                movementDir = moveTemp;
            }
            PlayerInteraction.AxisInput(context);
        }
        movementDir = moveTemp;

        if (movementDir.x != 0)
            lastMovementDir = movementDir.x;
    }

    public void PlayRightAnimation(float axis)
    {
        if (axis == 0)
            return;
        if (transform.position.x < PlayerInteraction.getInteractiveObjectPos.x && axis > 0 || (transform.position.x > PlayerInteraction.getInteractiveObjectPos.x && axis < 0))
        {
            if (!Controller.isPulling)
                StartCoroutine(Controller.PlayPush());
        }
        else
        {
            if (!Controller.isPushing)
                StartCoroutine(Controller.PlayPull());
        }
    }

    public void OnJump(CallbackContext context)
    {
        if ((PlayerActionState == PlayerAction.IDLE || PlayerActionState == PlayerAction.RUN) && PlayerInteraction.Interaction != PlayerInteraction.InteractionState.Link)
            if (context.performed)
                _isJumping = true;
    }

    public void OnTransformAction(CallbackContext context)
    {
        if (!context.performed || _isJumping || (PlayerActionState != PlayerAction.IDLE && PlayerActionState != PlayerAction.RUN) || PlayerAnimator.IsInAmination || PlayerInteraction.Interaction == PlayerInteraction.InteractionState.Link)
            return;
        if (_isShadow)
        {
            OnTransformToPlayer();
        }
        else
        {
            if (Caster.CanTransform(true))
            {
                OnTransformToShadow();
            }
        }
        _shadowEffectHandler.PlaySound();
    }

    public void OnTransformToShadow()
    {
        StartCoroutine(PlayerAnimator.TransformToShadowAnim());
        _isShadow = true;
        Controller.GroundType ^= LayerMask.GetMask("Shadows", "NoShadows");
    }

    public void OnTransformToPlayer()
    {
        PlayerAnimator.ShadowPosition = Caster.GetShadowPos();
        StartCoroutine(PlayerAnimator.TransformToPlayerAnim());
        _isShadow = false;
        Controller.GroundType ^= LayerMask.GetMask("Shadows", "NoShadows");
    }

    bool exactPos = false;
    public void OnInteract(CallbackContext context)
    {
        if (_isJumping || Controller.isClimbing || !Controller.IsGrounded || (PlayerActionState != PlayerAction.IDLE && PlayerActionState != PlayerAction.RUN && PlayerActionState != PlayerAction.INTERACT) || PlayerAnimator.IsInAmination || PlayerInteraction.Interaction == PlayerInteraction.InteractionState.None)
            return;
        if (PlayerInteraction.getInteractiveObjectPos.y + 0.25f < transform.position.y)
            return;
        PlayerActionState = PlayerAction.INTERACT;
        if (PlayerInteraction.getObjectType == "Box")
        {
            if (exactPos)
                PlayerInteraction.InteractionInput(context.started, context.canceled);
            else
                StartCoroutine(PlayAnimationBefore(context.started, context.canceled));
        }
        else
            PlayerInteraction.InteractionInput(context.started, context.canceled);

        if (PlayerInteraction.Interaction == PlayerInteraction.InteractionState.Selected)
        {
            PlayerActionState = PlayerAction.IDLE;
            exactPos = false;
        }

    }

    private void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Set Player to right Position
    IEnumerator PlayAnimationBefore(bool started, bool canceled)
    {
        Controller.canTurn = false;
        exactPos = true;
        float animationDistance = 0.65f;
        float Distance = (Vector3.Distance(transform.position, PlayerInteraction.getInteractiveObjectPos - (Vector3.right * (PlayerInteraction.getInteractiveObjectScale.x / 2 + animationDistance)) * Controller.Direction));
        float Distance2 = (Vector3.Distance(transform.position, PlayerInteraction.getInteractiveObjectPos - (Vector3.right * (PlayerInteraction.getInteractiveObjectScale.x / 2 + animationDistance)) * Controller.Direction * -1));
        if (Distance > Distance2)
        {
            Controller.FlipCharacter();
            yield return StartCoroutine(LerpTo(new Vector3(PlayerInteraction.getInteractiveObjectPos.x - ((PlayerInteraction.getInteractiveObjectScale.x / 2 + animationDistance)) * Controller.Direction * -1, transform.position.y, transform.position.z), 0.1f));
        }
        else if (Distance <= Distance2)
            yield return StartCoroutine(LerpTo(new Vector3(PlayerInteraction.getInteractiveObjectPos.x - ((PlayerInteraction.getInteractiveObjectScale.x / 2 + animationDistance)) * Controller.Direction, transform.position.y, transform.position.z), 0.1f));
        PlayerInteraction.InteractionInput(started, canceled);
        Controller.canTurn = true;
    }

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
        respawn = true;
        yield return _PauseMenu.ScreenfadeIn(1.0f,2.0f);
        Respawn();
    }
}
