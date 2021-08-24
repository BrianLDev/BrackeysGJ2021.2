using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent( typeof(Animator), typeof(PlayerInput), typeof(PlayerInputHandler) )]
public class BullController : MonoBehaviour {
    [SerializeField] float speed;
    private PlayerInputHandler inputHandler;
    private int animAttack, animIsEating, animIsWalking, animIsRunning, animIsDead;

    private void Awake() {
        inputHandler = GetComponentInChildren<PlayerInputHandler>();
        inputHandler = gameObject.AddComponent<PlayerInputHandler>();
        animAttack = Animator.StringToHash("Attack");   // TODO - NEED ATTACK ANIMATION FOR BULL
        animIsEating = Animator.StringToHash("isEating");
        animIsWalking = Animator.StringToHash("isWalking");
        animIsRunning = Animator.StringToHash("isRunning");
        animIsDead = Animator.StringToHash("isDead");
    }

    private void Update() {
        if (inputHandler.isJumping) {
            Debug.Log("Jumping! (handler)");
        }
    }
}