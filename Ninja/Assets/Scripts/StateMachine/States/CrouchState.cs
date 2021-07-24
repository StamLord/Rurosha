using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : State
{
    [Header("Control Settings")]
    [SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private float airControl = 5f;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private Vector3 croucingColliderSize;
    [SerializeField] private new CapsuleCollider collider;
    [Space(20f)]
	
    [SerializeField] private float gravity = 20.0f;
	[SerializeField] private float maxVelocityChange = 10.0f;

    [Header("Input Data")]
    [SerializeField] private InputState inputState;
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    
    [Space(20f)]
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundSlope;
    [SerializeField] private float groundSphereRadius = .4f;
    [SerializeField] private float groundDistance = .4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGrounded;

    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField]private new Rigidbody rigidbody;

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
        if(debugView) Debug.Log("State: Entered [Crouch State]");
        collider.height = croucingColliderSize.y;

        inputState = ((CharacterStateMachine)_stateMachine).inputState;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        GroundCheck();

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
        if (inputState.Crouch.State == VButtonState.UNPRESSED) 
            _stateMachine.SwitchState(0);

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
