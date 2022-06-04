using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleJumpState : PlayerState
{
    [Header("Jump Start Data")]
    [SerializeField] private float timeStamp;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravity = 20.0f;
    private float lastJumpHeight;
    private Vector3 originalSpeed;

    [Space(20f)]

    [Header("Control Settings")]
    [SerializeField] private AttributeDependant<float> JumpHeight;
    [SerializeField] private float airControl = 5f;
    
    [Space(20f)]

    [Header("Stats")]
    [SerializeField] private float potentialStaminaDepleteRate = 2f;
    [SerializeField] private float staminaDepleteRate = 20f;
    [SerializeField] private float agilityExpGain = .01f;

    [SerializeField] private float jumpVelocity = 10;
    [SerializeField] private float jumpMaxHeight = 2;
    [SerializeField] private float jumpMinHeight = 1;
    
    [Space(20f)]
    
    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private Vector3 targetVelocity;
    [SerializeField] private float maxVelocityChange = 10.0f;
    [Header("[IMPORTANT! Player needs this enabled]")]
    [SerializeField] private bool transformDirection;
    
    [Space(20f)]

    [Header("Wall Jumping")]
    [SerializeField] private bool wallJumpOn = true;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpMinimumSlope = -.1f;

    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;

    public delegate void OnJumpChargeDelegate(float percentage);
    public event OnJumpChargeDelegate OnJumpCharge;


	void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Simple Jump State]");

        // Jump
        if(IsGrounded)
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
        else
        {
            // Wall jump
            if (wallJumpOn && wallDetected && wallNormal.y >= wallJumpMinimumSlope)
            {
                rigidbody.velocity = wallNormal * wallJumpForce;
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
            }
            // Double Jump
            else
                rigidbody.velocity = new Vector3(rigidbody.velocity.x,  CalculateDoubleJumpVerticalSpeed(), rigidbody.velocity.z);
        }

        timeStamp = Time.time;
        originalSpeed =  new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        
        SetSoundType(StealthAgent.SoundType.NONE);
    }

    protected override void OnExitState()
    {
        base.OnExitState();
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        // Get duration of jump
        float completeTime = jumpMaxHeight / jumpVelocity;
        float minCompletion = jumpMinHeight / jumpVelocity;
        float completion = (Time.time - timeStamp) / completeTime;

        // Input
        inputVector = inputState.AxisInput;
        inputVector.Normalize();
        // Don't know why but the AxisInput from BTBrains doesn't need to be transformed
        targetDirection = (transformDirection) ? transform.TransformDirection(inputVector) : inputVector;
        Vector3 targetVelocity = targetDirection;
        targetVelocity *= airControl;
        Vector3 flatRigidVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = flatRigidVelocity - originalSpeed;
        Vector3 velocityChange = (targetVelocity - velocity);
        

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        // Clamp to air control to not gain speed on each jump (due to original speed)
        velocityChange = Vector3.ClampMagnitude(velocityChange, airControl);
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        
        if (rigidbody.velocity.y <= 0) 
        {
            if (IsGrounded)
                _stateMachine.SwitchState(0);
            else
                _stateMachine.SwitchState(5);
            return;
        }
        
        // Reset vertical velocity and switch to falling when jump button is not pressed and minimum time has passed
        if (inputState.Jump.State == VButtonState.UNPRESSED && rigidbody.velocity.y > 0 && completion > minCompletion)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
            _stateMachine.SwitchState(5);
            return;
        }

        if(gravityOn)
            rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
    }

    float CalculateJumpVerticalSpeed () 
    {
        float jumpHeight = JumpHeight.GetValue(characterStats);
        lastJumpHeight = jumpHeight;
        
        // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

    float CalculateDoubleJumpVerticalSpeed()
    {
        float jumpHeight = Mathf.Max(lastJumpHeight * .5f, JumpHeight.GetValueAt(0));
        return Mathf.Sqrt(2 * jumpHeight * gravity);;
    }
}
