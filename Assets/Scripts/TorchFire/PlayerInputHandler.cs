using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 moveDirectionRaw { get; private set; }
    public Vector2 lookDirectionRaw { get; private set; }
    public Vector3 moveDirection { get; private set; }
    public Vector3 lookDirection { get; private set; }
    public bool isSprinting { get; private set; }
    public bool isJumping { get; private set; }
    public bool isAttacking { get; private set; }
    public bool isDashing { get; private set; }
    public bool isSpecialPerforming { get; private set; }
    private PlayerInput playerInput;
    private string s_sprint = "Sprint";
    private string s_jump = "Jump";
    private string s_attack = "Attack";
    private string s_dash = "Dash";
    private string s_special = "Special";

    private void Awake() {
        playerInput = GetComponentInChildren<PlayerInput>();
    }

    public void OnMove(InputValue value) {
        moveDirectionRaw = value.Get<Vector2>();
        moveDirection = ConvertInputV2RawToV3(moveDirectionRaw);
    }

    public void OnLook(InputValue value) {
        lookDirectionRaw = value.Get<Vector2>();
        lookDirection = ConvertInputV2RawToV3(lookDirectionRaw);
    }

    public static Vector3 ConvertInputV2RawToV3(Vector2 inputRaw) {
        return new Vector3(inputRaw.x, 0, inputRaw.y);
    }

    private void Update() {
        if (playerInput.actions[s_sprint].IsPressed())
            isSprinting = true;
        else
            isSprinting = false;

        if (playerInput.actions[s_jump].WasPressedThisFrame())
            isJumping = true;
        else
            isJumping = false;

        if (playerInput.actions[s_attack].WasPressedThisFrame())
            isAttacking = true;
        else
            isAttacking = false;

        if (playerInput.actions[s_dash].WasPressedThisFrame())
            isDashing = true;
        else
            isDashing = false;

        if (playerInput.actions[s_special].IsPressed())
            isSpecialPerforming = true;
        else
            isSpecialPerforming = false;
    }

}
