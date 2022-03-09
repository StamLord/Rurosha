using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : State
{
    [Header("Control Settings")]
    [SerializeField] private float colliderHeight;
    [SerializeField] private new CapsuleCollider collider;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float airControl = 5f;
	[SerializeField] private float maxVelocityChange = 10.0f;
    [SerializeField] private Vector3 originalSpeed;

    [Space(20f)]
    [Header("Input Data")]
    [SerializeField] private InputState inputState;
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private Vector3 targetVelocity;

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
    [SerializeField] private float climbingStartDistannce = .75f;

    [Space(20f)]

    [Header("Stats")]
    [SerializeField] private CharacterStats characterStats;

    [Space(20f)]

    [Header("Glide")]
    [SerializeField] private bool glideOn = false;
    [SerializeField] private float glideGravityMultiplier = .2f;
    [SerializeField] private bool isGliding = false;

    [Space(20f)]
    [Header("Air Jumps")]
    [SerializeField] private int airJumps;
    [SerializeField] private int maxAirJumps = 2;

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
        if(debugView) Debug.Log("State: Entered [Air State]");

        characterStats = ((CharacterStateMachine)_stateMachine).characterStats;
        inputState = ((CharacterStateMachine)_stateMachine).inputState;

        originalSpeed =  new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        Vector3 inputVector = inputState.AxisInput;
        inputVector.Normalize();
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        GroundCheck();
        ClimbCheck();

        // Input
        GetInput();

        // Accelration Method
        //rigidbody.AddForce(targetVelocity * airControl, ForceMode.VelocityChange);
        
        // Instant Method
        
        targetVelocity *= airControl;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity - originalSpeed;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        if(gravityOn)
        {
            if(isGliding)
                rigidbody.AddForce(new Vector3 (0, -gravity * glideGravityMultiplier * rigidbody.mass, 0));
            else
                rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
        }

        // Jump
        if (inputState.Jump.State == VButtonState.PRESS_START && airJumps < maxAirJumps) 
        {
            airJumps++;
            _stateMachine.SwitchState(2);
        }

        // Switch to GroundedState
        if (isGrounded && rigidbody.velocity.y <= 0) 
        {
            airJumps = 0;
            _stateMachine.SwitchState(0);
        }

        // Switch to ClimbState
        if(isClimbing)
            _stateMachine.SwitchState(3);

    }

    private void GetInput()
    {
        inputVector = inputState.AxisInput;
        inputVector.Normalize();
        targetDirection = transform.TransformDirection(inputVector);
        targetVelocity = targetDirection;

        if(inputState.Crouch.State == VButtonState.PRESSED && glideOn && isGliding == false)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
            isGliding = true;
        }
        else if(inputState.Crouch.State == VButtonState.UNPRESSED && isGliding)
            isGliding = false;
    }

    #region Checks

    private void GroundCheck()
    {
        RaycastHit groundHit;
        isGrounded = Physics.SphereCast(groundCheck.position, groundSphereRadius, Vector3.down, out groundHit, groundDistance, groundMask);
        groundSlope = Vector3.Angle(groundHit.normal, Vector3.up);
    }

    private void ClimbCheck()
    {
        isClimbing = (Physics.Raycast(climbCheck.position, targetDirection, climbingStartDistannce, climbMask));
    }

    #endregion
}
