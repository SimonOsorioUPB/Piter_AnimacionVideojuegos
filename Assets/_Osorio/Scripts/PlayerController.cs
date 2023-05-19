using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum States { idle, moving, attacking_1, attacking_2, attacking_3 }

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private ThirdPersonCam thirdPersonCam;
    
    [Header("Jump")]
    [SerializeField] private float jumpCooldown;
    [SerializeField] private bool readyToJump;

    [Header("Hurt")]
    [SerializeField] private float hurtCooldown;
    [SerializeField] private bool readyToHurt;
    
    [Header("Die")]
    [SerializeField] private float dieCooldown;
    [SerializeField] private bool readyToDie;

    [Header("Idle Breaker")] 
    [SerializeField] private float timeToBreakIdle = 10f;
    private float idleBreakerTimer = 0f;
    private bool idleBreaking;
    
    [Header("Animator")]
    [SerializeField] private Animator animator;
    
    private Rigidbody rb;
    private InputController inputActions;
    private Vector3 moveDirection;
    private States currentState;
    private bool attackStarted;

    [SerializeField] private float chainAttackTime = 1;
    private float chainAttackTimer = 0;

    private float stopAttackTimer;
    private float stopAttackTime = 2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        inputActions = thirdPersonCam.InputActions;
        currentState = States.idle;
        readyToJump = true;
        readyToDie = true;
        readyToHurt = true;
        attackStarted = false;
        inputActions.Player.Attack.performed += AttackPressed;
        idleBreaking = false;
    }

    private void Update()
    {
        switch (currentState)
        {
            default:
            case States.idle: HandleIdle(); HandleTriggers(); break;
            case States.moving: HandleMovement(); HandleTriggers(); break;
            case States.attacking_1: HandleTriggers(); break;
        }

        chainAttackTimer -= Time.deltaTime;
        if (chainAttackTimer < 0) chainAttackTimer = 0;
    }

    private void HandleTriggers()
    {
        HandleJump();
        HandleHurt();
        HandleDie();
        HandleAttack();
    }

    private void HandleIdle()
    {
        animator.SetTrigger("StopAttack");
        Vector2 movementInput = inputActions.Player.Movement.ReadValue<Vector2>();
        animator.SetFloat("Velocity", 0);
        if (movementInput != Vector2.zero) currentState = States.moving;
        if (idleBreakerTimer >= timeToBreakIdle)
        {
            if (!idleBreaking) { animator.SetTrigger("IdleBreaker"); idleBreaking = true; }
            if (idleBreakerTimer >= timeToBreakIdle+2f) { idleBreakerTimer = 0; idleBreaking = false; }
        }
        idleBreakerTimer += Time.deltaTime;
    }    
    private void HandleMovement()
    {
        Vector2 movementInput = inputActions.Player.Movement.ReadValue<Vector2>();
        moveDirection = orientation.forward * movementInput.y + orientation.right * movementInput.x;
        float speed = moveSpeed;
        if (inputActions.Player.Sprint.IsPressed())
        {
            animator.SetBool("IsSprinting", true);
            speed = sprintSpeed;
        }
        else animator.SetBool("IsSprinting", false);
            
        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        
        animator.SetFloat("Velocity", 1);
        if (movementInput == Vector2.zero) currentState = States.idle;
    }

    private void HandleJump()
    {
        if (inputActions.Player.Jump.IsPressed() && readyToJump)
        {
            readyToJump = false;
            animator.SetTrigger("Jump");
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void HandleHurt()
    {
        if (inputActions.Player.GetHit.IsPressed() && readyToHurt)
        {
            readyToHurt = false;
            animator.SetTrigger("TakeDamage");
            Invoke(nameof(ResetHurt), hurtCooldown);
        }
    }
    
    private void HandleDie()
    {
        if (inputActions.Player.Die.IsPressed() && readyToDie)
        {
            readyToDie = false;
            animator.SetTrigger("Die");
            Invoke(nameof(ResetDie), dieCooldown);
        }
    }
    
    private void HandleAttack()
    {
        if (currentState == States.idle || currentState == States.moving) { if (attackStarted) Attack(1); }
        else if (chainAttackTimer <= 0)
        {
            currentState = States.idle; animator.SetTrigger("StopAttack"); 
            animator.SetBool("IsAttacking", false);
        }
        else if (chainAttackTimer > 0)
        {
            if (currentState == States.attacking_1) { if (attackStarted) Attack(2); }
            if (currentState == States.attacking_2) { if (attackStarted) Attack(3); }
            if (currentState == States.attacking_3) { if (attackStarted) Attack(1); }
        }
    }

    private void ResetJump() { readyToJump = true; }
    private void ResetHurt() { readyToHurt = true; }
    private void ResetDie()  { readyToDie = true; }

    private void Attack(int attack)
    {
        attackStarted = false;
        chainAttackTimer = chainAttackTime;
        if (attack == 1) { animator.SetTrigger("Attack_01"); currentState = States.attacking_1; }
        if (attack == 2) { animator.SetTrigger("Attack_02"); currentState = States.attacking_2; }
        if (attack == 3) { animator.SetTrigger("Attack_03"); currentState = States.attacking_3; }
    }

    private void AttackPressed(InputAction.CallbackContext ctx)
    {
        if (!attackStarted) attackStarted = true;
    }
}
