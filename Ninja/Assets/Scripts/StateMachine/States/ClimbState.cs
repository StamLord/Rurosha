using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbState : State
{
    [Header("Control Settings")]
    [SerializeField] private float climbSpeed = 4.0f;
    [SerializeField] private Vector3 climbingColliderSize;
    [SerializeField] private new CapsuleCollider collider;
    [Space(20f)]
	
	[SerializeField] private float maxVelocityChange = 10.0f;

    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private Vector3 targetVelocity;
    
    [Space(20f)]
    
    [Header("Climb Detection")]
    [SerializeField] private Transform climbCheck;
    [SerializeField] private LayerMask climbMask;
    [SerializeField] private bool isClimbing;

    [Space(20f)]

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundSlope;
    [SerializeField] private float groundSphereRadius = .4f;
    [SerializeField] private float groundDistance = .8f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGrounded;

    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField]private new Rigidbody rigidbody;

    [SerializeField]private PhysicMaterial noFriction;
    [SerializeField]private PhysicMaterial friction;

    void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;

        collider = GetComponent<CapsuleCollider>();
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Climb State]");
        collider.height = climbingColliderSize.y;

        collider.material = friction;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        GetInput();

        targetVelocity *= climbSpeed;

        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);

        velocityChange.x = 0;
        velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = 0;
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        ClimbCheck();

        if(ClimbDownCheck())
            _stateMachine.SwitchState(0);

        
        if(Input.GetButtonDown("Jump"))
            _stateMachine.SwitchState(2);

        // Jump off
        if(Input.GetButtonDown("Jump"))
            _stateMachine.SwitchState(2);

        // Fall if no longer climbing
        if(isClimbing == false)
            _stateMachine.SwitchState(2);

    }

    protected override void OnExitState()
    {
        base.OnExitState();

        collider.material = noFriction;
    }

    private void GetInput()
    {
        inputVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        inputVector.Normalize();
        targetDirection = transform.TransformDirection(inputVector);
        targetVelocity = targetDirection;
    }

    private void ClimbCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(climbCheck.position, 1f, climbMask);
        isClimbing = (colliders.Length > 0);
    }

    private bool ClimbDownCheck()
    {
        if(inputVector.y > 0) return false;

        RaycastHit groundHit;
        isGrounded = Physics.SphereCast(groundCheck.position, groundSphereRadius, targetDirection, out groundHit, groundDistance, groundMask);
        groundSlope = Vector3.Angle(groundHit.normal, Vector3.up);

        return isGrounded;
    }
}
