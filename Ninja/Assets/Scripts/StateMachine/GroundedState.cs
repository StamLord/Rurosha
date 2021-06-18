﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class GroundedState : State
{
    [Header("Control Settings")]
    [SerializeField] private float walkSpeed = 10.0f;
    // [SerializeField] private float runSpeed = 15.0f;
    // [SerializeField] private float[] runSpeedPerAgilityLevel = {10, 10.5f, 11, 11.5f, 12, 12.5f, 13, 13.5f, 14, 15.0f};
    [SerializeField] private AttributeDependant<float> _runSpeed;
    [SerializeField] private float airControl = 5f;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private Vector3 standingColliderSize;
    [SerializeField] private new CapsuleCollider collider;
    
    [Space(20f)]
    [Header("Stats")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private float staminaDepleteRate = 20f;
    [SerializeField] private float potentialStaminaDepleteRate = 2f;
    [SerializeField] private float enduranceExpGain = .01f;
    [Space(20f)]
	
    [SerializeField] private float gravity = 20.0f;
	[SerializeField] private float maxVelocityChange = 10.0f;

    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    
    [Space(20f)]
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundSlope;
    [SerializeField] private float groundSphereRadius = .4f;
    [SerializeField] private float groundDistance = .8f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGrounded;
    public bool IsGrounded {get{return isGrounded;}}

    [Space(20f)]

    [Header("Climb Detection")]
    [SerializeField] private Transform climbCheck;
    [SerializeField] private LayerMask climbMask;
    [SerializeField] private bool isClimbing;

    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;

    void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity = false;

        collider = GetComponent<CapsuleCollider>();
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Ground State]");
        collider.height = standingColliderSize.y;
        characterStats = ((PlayerControls)_stateMachine).characterStats;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        GroundCheck();
        ClimbCheck();

        // Input
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputVector.Normalize();
        targetDirection = transform.TransformDirection(inputVector);
        Vector3 targetVelocity = targetDirection;

        // Ground Control
	    if (isGrounded) 
        {
            if(Input.GetButton("Run"))
            {
                if(characterStats.DepleteStamina(staminaDepleteRate * Time.deltaTime))
                {
                    characterStats.IncreaseAttributeExp("endurance", enduranceExpGain * Time.deltaTime);
                    characterStats.DepletePotentailStamina(potentialStaminaDepleteRate * Time.deltaTime);

                    // Set relevant speed
                    //targetVelocity *= runSpeedPerAgilityLevel[characterStats.GetAttributeLevel("agility") - 1];
                    targetVelocity *= _runSpeed.GetValue(characterStats);

                }
                else
                    targetVelocity *= walkSpeed;
            }
            else
                targetVelocity *= walkSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            if(Input.GetKeyDown(KeyCode.X) && characterStats.DepleteStamina(20))
                _stateMachine.SwitchState(4);
        }
        // Air
        //else 
            //rigidbody.AddForce(targetVelocity * airControl, ForceMode.Acceleration);

        // Kick
        if (Input.GetKeyDown(KeyCode.F)) 
            GetComponent<Kick>().ActivateKick();

        // Jump
        if (Input.GetButton("Jump")) 
        {
            if(isGrounded)
                _stateMachine.SwitchState(2);
        }

        // Crouch
        if (Input.GetKey(KeyCode.C)) 
            _stateMachine.SwitchState(1);
        
        if(isClimbing)
            _stateMachine.SwitchState(3);

	    // We apply gravity manually for more tuning control
        if(gravityOn)
	        rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
    }

    private void GroundCheck()
    {
        RaycastHit groundHit;
        isGrounded = Physics.SphereCast(groundCheck.position, groundSphereRadius, Vector3.down, out groundHit, groundDistance, groundMask);
        groundSlope = Vector3.Angle(groundHit.normal, Vector3.up);
    }

    private void ClimbCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(climbCheck.position, 1f, climbMask);
        isClimbing = (colliders.Length > 0 && inputVector.z > 0);
    }

    private void OnDrawGizmos()
    {
        if(!debugView) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, groundSphereRadius);
        Gizmos.DrawWireSphere(groundCheck.position + Vector3.down * groundDistance, groundSphereRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, targetDirection);
    }
}
