using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : State
{
    [Header("Jump Start Data")]
    [SerializeField] private float _timeStamp;
    [SerializeField] private Vector3 _jumpDirection;
    [SerializeField] private bool _fromGround;

    [Space(20f)]

    [Header("Control Settings")]
    //[SerializeField] private float jumpHeight = 2.0f;
    //[SerializeField] private float[] jumpHeightPerAgilityLevel = {.75f, 1f, 1.25f, 1.5f, 2f, 3f, 4f, 5f, 6f, 8f};
    [SerializeField] private AttributeDependant<float> JumpHeight;
    [SerializeField] private float airControl = 5f;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private bool gravityOn = true;
	[SerializeField] private float maxVelocityChange = 10.0f;
    [SerializeField] private Vector3 originalSpeed;
    
    [Space(20f)]

    [Header("Stats")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private float staminaDepleteRate = 20f;
    [SerializeField] private float enduranceExpGain = .01f;
    
    [Space(20f)]
    
    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private Vector3 targetVelocity;
    
    [Space(20f)]
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundSlope;
    [SerializeField] private float groundSphereRadius = .4f;
    [SerializeField] private float groundDistance = .4f;
    [SerializeField] private LayerMask groundMask;
    
    [Space(20f)]

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpWindow= .2f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private bool wallJumpOn = true;
    [SerializeField] private Transform wallJumpCheck;
    [SerializeField] private float wallJumpDistance = .7f;
    [SerializeField] private float wallJumpMinimumSlope = -.1f;
    [SerializeField] private LayerMask wallJumpMask;
    [SerializeField] private GameObject wallCollided;
    [SerializeField] private Vector3 wallHitNormal;

    [Space(20f)]

    [Header("Glide")]

    [SerializeField] private bool glideOn = false;
    [SerializeField] private float glideGravityMultiplier = .2f;
    [SerializeField] private bool isGliding = false;

    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new CapsuleCollider collider;
 
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
        if(debugView) Debug.Log("State: Entered [Jump State]");

        characterStats = ((CharacterStateMachine)_stateMachine).characterStats;

        _timeStamp = Time.time;

        GetInput();
        _jumpDirection = targetDirection;

        _fromGround = isGrounded();
        
        originalSpeed =  new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        if(_fromGround)
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
        else if (wallJumpOn)
        {
            WallJumpCheck();
            if(wallCollided != null && wallHitNormal.y >= wallJumpMinimumSlope)
            {
                rigidbody.velocity = wallHitNormal * wallJumpForce;
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
                wallCollided = null;
                return;
            }
        }
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        // Input
        GetInput();

        // Accelration Method
        rigidbody.AddForce(targetVelocity * airControl, ForceMode.VelocityChange);
        
        // Instant Method
        /*
        targetVelocity *= airControl;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity - originalSpeed;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        // if(dotProduct == 1)
        // {
        //     velocityChange += projectedInput;
        // }
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);*/
        
        if(Input.GetButtonDown("Jump"))
            _stateMachine.SwitchState(2);

        if(gravityOn)
        {
	        if(isGliding)
                rigidbody.AddForce(new Vector3 (0, -gravity * glideGravityMultiplier * rigidbody.mass, 0));
            else
                rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
        }

        if(isGrounded() && rigidbody.velocity.y < 0.1f)
            _stateMachine.SwitchState(0);
    }

    float CalculateJumpVerticalSpeed () 
    {
	    // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
	    //return Mathf.Sqrt(2 * jumpHeightPerAgilityLevel[characterStats.GetAttributeLevel("agility") - 1] * gravity);
        return Mathf.Sqrt(2 * JumpHeight.GetValue(characterStats) * gravity);
	}

    private void GetInput()
    {
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputVector.Normalize();
        targetDirection = transform.TransformDirection(inputVector);
        targetVelocity = targetDirection;

        if(Input.GetButton("Crouch") && glideOn && isGliding == false)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
            isGliding = true;
        }
        else if(Input.GetButton("Crouch") == false && isGliding)
            isGliding = false;
    }

    private bool isGrounded()
    {
        RaycastHit groundHit;
        bool _isGrounded = Physics.SphereCast(groundCheck.position, groundSphereRadius, Vector3.down, out groundHit, groundDistance, groundMask);
        groundSlope = Vector3.Angle(groundHit.normal, Vector3.up);
        return _isGrounded;
    }

    private void WallJumpCheck()
    {
        wallCollided = null;

        if(Time.time - _timeStamp > wallJumpWindow || _fromGround) return;
        
        RaycastHit wall;

        if(Physics.Raycast(wallJumpCheck.position, _jumpDirection, out wall, wallJumpDistance, wallJumpMask) /*||
            Physics.Raycast(wallJumpCheck.position, rigidbody.velocity, out wall, wallJumpDistance, wallJumpMask)*/)
        {
            wallCollided = wall.transform.gameObject;
            wallHitNormal = wall.normal;
        }
    }
}
