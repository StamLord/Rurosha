using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : State
{
    [Header("Jump Start Data")]
    [SerializeField] private float _timeStamp;
    [SerializeField] private Vector3 _jumpDirection;
    [SerializeField] private bool _fromGround;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravity = 20.0f;

    [Space(20f)]

    [Header("Control Settings")]
    [SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private AttributeDependant<float> _runSpeed;
    [SerializeField] private AttributeDependant<float> JumpHeight;
    [SerializeField] private bool chargeJump = true;
    
    [Space(20f)]

    [Header("Stats")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private InputState inputState;
    [SerializeField] private float potentialStaminaDepleteRate = 2f;
    [SerializeField] private float staminaDepleteRate = 20f;
    [SerializeField] private float enduranceExpGain = .01f;
    
    [Space(20f)]
    
    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private Vector3 targetVelocity;
    [SerializeField] private float maxVelocityChange = 10.0f;
    [Header("[IMPORTANT! Player needs this enabled]")]
    [SerializeField] private bool transformDirection;
    [SerializeField] private float pressTime;
    [SerializeField] private float maxPressTime = 1f;
    
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
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private new CapsuleCollider collider;

    public delegate void OnJumpChargeDelegate(float percentage);
    public event OnJumpChargeDelegate OnJumpCharge;

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
        inputState = ((CharacterStateMachine)_stateMachine).inputState;
        
        _timeStamp = Time.time;

        _jumpDirection = targetDirection;
        
        GroundCheck();
        _fromGround = isGrounded;

        if(_fromGround == false)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x,  CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
        }
        
        if (_fromGround == false && wallJumpOn)
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

        GroundCheck();

        // Input
        inputVector = inputState.AxisInput;
        inputVector.Normalize();
        // Don't know why but the AxisInput from BTBrains doesn't need to be transformed
        targetDirection = (transformDirection) ? transform.TransformDirection(inputVector) : inputVector;
        Vector3 targetVelocity = targetDirection;

        // Ground Control
        if (isGrounded) 
        {
            if(inputState.Run.State == VButtonState.PRESSED)
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

            if(inputState.DoubleForward || inputState.DoubleBack || inputState.DoubleLeft || inputState.DoubleRight)
                if(characterStats.DepleteStamina(20))
                    _stateMachine.SwitchState(4);

            // Update jump charge listeners
            if(OnJumpCharge != null)
                OnJumpCharge(pressTime / maxPressTime);

            // Release of jump button
            if (inputState.Jump.State == VButtonState.UNPRESSED && isGrounded) 
            {    
                rigidbody.velocity = new Vector3(rigidbody.velocity.x,  CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
                pressTime = 0f;
                if(OnJumpCharge != null)
                    OnJumpCharge(-1f);
            }

            if(gravityOn)
                rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
        }
        // In Air
        else
        {
            _stateMachine.SwitchState(5);
        }

        pressTime = Mathf.Min(inputState.Jump.PressTime, maxPressTime);
    }

    float CalculateJumpVerticalSpeed () 
    {
        float jumpHeight = JumpHeight.GetValue(characterStats);
        if(chargeJump) 
        {
            jumpHeight *= (pressTime / maxPressTime);
            jumpHeight = Mathf.Max(JumpHeight.GetValueAt(0), jumpHeight); // Jump height must be atleast the height at level 1
        }
        
        // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

    
    private void GroundCheck()
    {
        RaycastHit groundHit;
        isGrounded = Physics.SphereCast(groundCheck.position, groundSphereRadius, Vector3.down, out groundHit, groundDistance, groundMask);
        groundSlope = Vector3.Angle(groundHit.normal, Vector3.up);
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
