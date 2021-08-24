using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { MOVEMENT, ATTACK, DASH, SPECIAL}

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float moveSpeed = 5.0f;
    public float sprintSpeed = 8.0f;
    private Vector3 pointToLook;


    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;
    [Tooltip("Time required to pass before being able to attack again. Set to 0f to instantly attack again")]
    public float AttackTimeout = 0.50f;
    [Tooltip("Time required to pass before being able to dash again. Set to 0f to instantly dash again")]
    public float DashTimeout = 3.0f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    // private PlayerInputScript _input_old;
    private PlayerInputHandler _input;
    private CharacterController _controller;
    private Animator _anim;
    private PlayerState playerstate;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _attackTimeoutDelta;
    private float _dashTimeoutDelta;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private int attckState = 0;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<PlayerInputHandler>();
        _controller = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        playerstate = PlayerState.MOVEMENT;
    }

    // Update is called once per frame
    void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
        CalculateRotation();
        Attack();
        Dash();
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmos()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, GroundedRadius);
    }

    private void Move()
    {
        float targetSpeed = _input.isSprinting ? sprintSpeed : moveSpeed;

        if (_input.moveDirectionRaw == Vector2.zero || playerstate != PlayerState.MOVEMENT) targetSpeed = 0.0f;

        _controller.Move(_input.moveDirection * targetSpeed * Time.deltaTime);
        _controller.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        if(_anim != null)
        {
            _anim.SetFloat("Speed", targetSpeed);
        }
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.isJumping && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            //_input.isJumping = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private void CalculateRotation()
    {
        Vector3 mousePosition = _input.lookDirectionRaw;
        Ray cameraRay = Camera.main.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        float rayLength;
        if (groundPlane.Raycast(cameraRay, out rayLength) && playerstate == PlayerState.MOVEMENT)
        {
            pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.red);
            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }

    private void Attack()
    {
        if (_input.isAttacking && _attackTimeoutDelta <= 0.0f)
        {
            _attackTimeoutDelta = AttackTimeout;
            playerstate = PlayerState.ATTACK;
            _anim.SetInteger("Attack", ++attckState);
        }

        if(_attackTimeoutDelta >= 0.0f)
        {
            _attackTimeoutDelta -= Time.deltaTime;
        }
    }

    private void Dash()
    {
        if(_input.isDashing && _dashTimeoutDelta <= 0.0f && playerstate == PlayerState.MOVEMENT)
        {
            _dashTimeoutDelta = DashTimeout;
            playerstate = PlayerState.DASH;
            StartCoroutine(DashCoroutine());

        }
        if (_dashTimeoutDelta >= 0.0f)
        {
            _dashTimeoutDelta -= Time.deltaTime;
            Debug.Log(_dashTimeoutDelta);
        }
    }

    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time;
        while (Time.time < startTime + 0.2f)
        {
            _controller.Move(transform.forward * 50 * Time.deltaTime);
            yield return null;
        }
        playerstate = PlayerState.MOVEMENT;
    }

    public void ResetAttack()
    {
        attckState = 0;
        playerstate = PlayerState.MOVEMENT;
        _anim.SetInteger("Attack", attckState);
    }
}
