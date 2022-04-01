﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private float airControl = 5f;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private Vector3 standingColliderSize;
    [SerializeField] private Vector3 croucingColliderSize;
    [SerializeField] private CapsuleCollider standCollider;
    [SerializeField] private CapsuleCollider crouchCollider;
    
    [SerializeField] private float gravity = 20.0f;
	[SerializeField] private float maxVelocityChange = 10.0f;
    
    [Space(20f)]

    [Header("Stealth")]
    [SerializeField] private float crouchVisibility = .7f;
    [SerializeField] private float crouchDetection = .5f;
    
    [Space(20f)]

    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;

    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField]private new Rigidbody rigidbody;

    public delegate void CrouchStartDelegate();
    public event CrouchStartDelegate OnCrouchStart;

    public delegate void CrouchEndDelegate();
    public event CrouchEndDelegate OnCrouchEnd;

    void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity = false;
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Crouch State]");
        
        crouchCollider.enabled = true;
        standCollider.enabled = false;

        SetVisibility(crouchVisibility);
        SetDetection(crouchDetection);

        if(OnCrouchStart != null) OnCrouchStart();
    }

    protected override void OnExitState()
    {
        base.OnExitState();
        crouchCollider.enabled = false;
        standCollider.enabled = true;

        SetVisibility(1f);
        SetDetection(1f);

        if(OnCrouchEnd != null) OnCrouchEnd();
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        // Input
        inputVector = inputState.AxisInput;
        inputVector.Normalize();
        targetDirection = transform.TransformDirection(inputVector);
        Vector3 targetVelocity = targetDirection;

        // Ground Control
	    if (isGrounded) 
        {
            targetVelocity *= walkSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        // Air
        else 
            rigidbody.AddForce(targetVelocity * airControl, ForceMode.Acceleration);

        // Crouch
        if (inputState.Crouch.State == VButtonState.UNPRESSED && isUnder == false)
            _stateMachine.SwitchState(0);

	    // We apply gravity manually for more tuning control
        if(gravityOn)
	        rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
    }

    private void OnDrawGizmos()
    {
        if(!debugView) return;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, targetDirection);
    }
}
