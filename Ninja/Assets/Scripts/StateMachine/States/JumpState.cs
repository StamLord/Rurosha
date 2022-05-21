using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{
    [Header("Jump Start Data")]
    [SerializeField] private float _timeStamp;
    [SerializeField] private Vector3 _jumpDirection;
    [SerializeField] private bool _fromGround;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravity = 20.0f;
    private float lastJumpHeight;

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
    [SerializeField] private float agilityExpGain = .01f;
    
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

    [Header("Wall Jumping")]
    [SerializeField] private bool wallJumpOn = true;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpMinimumSlope = -.1f;

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
        _fromGround = IsGrounded;

        if(_fromGround == false)
        {
            if (wallJumpOn && wallDetected && wallNormal.y >= wallJumpMinimumSlope)
            {
                rigidbody.velocity = wallNormal * wallJumpForce;
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
                _stateMachine.SwitchState(5);
                return;
            }
            else
            {
                rigidbody.velocity = new Vector3(rigidbody.velocity.x,  CalculateDoubleJumpVerticalSpeed(), rigidbody.velocity.z);
                _stateMachine.SwitchState(5);
            }
        }
    }

    protected override void OnExitState()
    {
        base.OnExitState();
        ResetCharging();
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        // Input
        inputVector = inputState.AxisInput;
        inputVector.Normalize();
        // Don't know why but the AxisInput from BTBrains doesn't need to be transformed
        targetDirection = (transformDirection) ? transform.TransformDirection(inputVector) : inputVector;
        Vector3 targetVelocity = targetDirection;

        // Ground Control
        if (IsGrounded) 
        {
            if(inputState.Run.State == VButtonState.PRESSED)
            {
                if(characterStats.DepleteStamina(staminaDepleteRate * Time.deltaTime))
                {
                    characterStats.IncreaseAttributeExp("agility", agilityExpGain * Time.deltaTime);
                    characterStats.DepletePotentailStamina(potentialStaminaDepleteRate * Time.deltaTime);

                    // Set relevant speed
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
            if (inputState.Jump.State == VButtonState.UNPRESSED && IsGrounded) 
            {    
                rigidbody.velocity = new Vector3(rigidbody.velocity.x,  CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
                ResetCharging();
                
                // Transition to Air State
                 _stateMachine.SwitchState(5);
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

    private void ResetCharging()
    {
        pressTime = 0f;
        if(OnJumpCharge != null)
            OnJumpCharge(-1f);
    }

    float CalculateJumpVerticalSpeed () 
    {
        float jumpHeight = JumpHeight.GetValue(characterStats);
        if(chargeJump) 
        {
            float level1 = JumpHeight.GetValueAt(0);
            float p = pressTime / maxPressTime;

            if(level1 == jumpHeight)
                jumpHeight = Mathf.Lerp(level1, level1 * 1.5f, p);
            else
                jumpHeight = Mathf.Lerp(level1, jumpHeight, p);
        }
        
        // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
        lastJumpHeight = jumpHeight;
        return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

    float CalculateDoubleJumpVerticalSpeed()
    {
        float jumpHeight = Mathf.Max(lastJumpHeight * .5f, JumpHeight.GetValueAt(0));
        return Mathf.Sqrt(2 * jumpHeight * gravity);;
    }
}
