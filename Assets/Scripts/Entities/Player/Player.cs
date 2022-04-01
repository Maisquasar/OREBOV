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
    float shadowTime;
    bool isJumping = false;
    Vector2 movementDir;

    // Update is called once per frame
    void Update()
    {
        Controller.Move(movementDir.x * speed, isJumping);
        if (isJumping)
            isJumping = false;
    }

    private void FixedUpdate()
    {
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
                isJumping = true;
    }
}
