using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;
using States;


public class Player : Entity
{
    public PlayerMovement Controller;
    public ShadowCaster Caster;
    public PlayerAnimator PlayerAnimator;
    float _shadowTime;
    bool _isJumping = false;
    bool _isShadow = false;
    Vector2 movementDir;
    Vector3 previousPos;

    // Update is called once per frame
    void Update()
    {
        if (!PlayerAnimator.IsInAmination) Controller.Move(movementDir.x * speed, _isJumping);
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
    }

    public void OnJumping(CallbackContext context)
    {
        if (Controller.PlayerActionState == PlayerAction.IDLE || Controller.PlayerActionState == PlayerAction.RUN)
            if (context.performed)
                _isJumping = true;
    }

    public void OnTransformAction(CallbackContext context)
    {
        if (!context.performed || _isJumping || (Controller.PlayerActionState != PlayerAction.IDLE && Controller.PlayerActionState != PlayerAction.RUN) || PlayerAnimator.IsInAmination)
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
}
