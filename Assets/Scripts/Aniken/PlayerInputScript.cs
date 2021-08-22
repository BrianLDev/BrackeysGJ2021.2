using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputScript : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnLook(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }
}
