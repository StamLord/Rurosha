using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class GroundedState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private AttributeDependant<float> _runSpeed;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private Vector3 standingColliderSize;
    [SerializeField] private new CapsuleCollider collider;
    
    [Space(20f)]
    
    [Header("Stats")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private float staminaDepleteRate = 20f;
    [SerializeField] private float potentialStaminaDepleteRate = 2f;
    [SerializeField] private float enduranceExpGain = .01f;
    [SerializeField] private float gravity = 20.0f;

    [Header("Input Data")]
    [SerializeField] private InputState inputState;
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
	[SerializeField] private float maxVelocityChange = 10.0f;
    [Header("[IMPORTANT! Player needs this enabled]")]
    [SerializeField] private bool transformDirection;
    
    [Space(20f)]

    [Header("Climb Detection")]
    [SerializeField] private Transform climbCheck;
    [SerializeField] private LayerMask climbMask;
    [SerializeField] private bool isClimbing;
    [SerializeField] private float climbingStartDistannce = .75f;

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
        characterStats = ((CharacterStateMachine)_stateMachine).characterStats;
        inputState = ((CharacterStateMachine)_stateMachine).inputState;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        ClimbCheck();

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
        }

        // Kick
        if (Input.GetKeyDown(KeyCode.F)) 
            GetComponent<Kick>().ActivateKick();

        // Jump
        if (inputState.Jump.Pressed) 
        {
            if(IsGrounded)
                _stateMachine.SwitchState(2);
        }

        // Crouch
        if (inputState.Crouch.State == VButtonState.PRESSED) 
            _stateMachine.SwitchState(1);
        
        if(isClimbing)
            _stateMachine.SwitchState(3);

        // Switch to AirSTate
        if (IsGrounded == false) 
            _stateMachine.SwitchState(5);
        
        // Switch to ClimbState
        if(isClimbing)
            _stateMachine.SwitchState(3);

	    // We apply gravity manually for more tuning control
        if(gravityOn)
	        rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
    }

    #region Checks

    private void ClimbCheck()
    {
        isClimbing = (Physics.Raycast(climbCheck.position, targetDirection, climbingStartDistannce, climbMask));
    }

    #endregion

    private void OnDrawGizmos()
    {
        if(debugView == false) return;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, targetDirection);
    }
}
