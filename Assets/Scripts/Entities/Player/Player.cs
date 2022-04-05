using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;
using States;


public class Player : Entity
{
    [HideInInspector] public PlayerAction PlayerActionState;
    private PlayerMovement Controller;
    private ShadowCaster Caster;
    private PlayerAnimator PlayerAnimator;
    private PlayerInteraction PlayerInteraction;
    float _shadowTime;
    bool _isJumping = false;
    bool _isShadow = false;
    Vector2 movementDir;
    Vector3 previousPos;

    private void Start()
    {
        Controller = gameObject.GetComponent<PlayerMovement>();
        Caster = gameObject.GetComponent<ShadowCaster>();
        PlayerAnimator = gameObject.GetComponent<PlayerAnimator>();
        PlayerInteraction = gameObject.GetComponent<PlayerInteraction>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!PlayerAnimator.IsInAmination && PlayerInteraction.Interaction != PlayerInteraction.InteractionState.Link)
        {
            Controller.Move(movementDir.x * speed, _isJumping);
            Controller.ChangeState(ref PlayerActionState);
        }
        if (_isJumping)
            _isJumping = false;
        if (_isShadow && !Caster.CanTransform())
        {
            if (Caster.DoesCurrentLightEject)
            {
                OnTransformToPlayer();
            }
            else
            {
                Vector3 pos = transform.position;
                transform.position = new Vector3(previousPos.x,transform.position.y,previousPos.z);
                if (!Caster.CanTransform())
                {
                    transform.position = pos;
                    OnTransformToPlayer();
                }
            }
        }
        previousPos = transform.position;
    }

    public void OnMove(CallbackContext context)
    {
        movementDir = context.ReadValue<Vector2>();
        if (Mathf.Abs(movementDir.x) < 0.03f) movementDir.x = 0.0f;
        if (Mathf.Abs(movementDir.y) < 0.03f) movementDir.y = 0.0f;
        PlayerInteraction.AxisInput(context);
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
            if (Caster.CanTransform()) OnTransformToShadow();
        }
    }

    public void OnTransformToShadow()
    {
        StartCoroutine(PlayerAnimator.TransformToShadowAnim());
        _isShadow = true;
        Controller.GroundType ^= LayerMask.GetMask("Shadows","NoShadows");
    }

    public void OnTransformToPlayer()
    {
        PlayerAnimator.ShadowPosition = Caster.GetShadowPos();
        StartCoroutine(PlayerAnimator.TransformToPlayerAnim());
        _isShadow = false;
        Controller.GroundType ^= LayerMask.GetMask("Shadows", "NoShadows");
    }

    public void OnInteract(CallbackContext context)
    {
        if (_isJumping || (PlayerActionState != PlayerAction.IDLE && PlayerActionState != PlayerAction.RUN) || PlayerAnimator.IsInAmination)
            return;
        PlayerInteraction.InteractionInput(context);
    }
}
