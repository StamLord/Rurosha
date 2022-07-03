using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]

public class GroundedState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private AttributeDependant<float> _runSpeed;
    [SerializeField] private float slopeSlideSpeed = 10.0f;
    [SerializeField] private float minSlideAngle = 30f;
    [SerializeField] private bool gravityOn = true;
    
    [Space(20f)]
    
    [Header("Stats")]
    [SerializeField] private float staminaDepleteRate = 20f;
    [SerializeField] private float potentialStaminaDepleteRate = 2f;
    [SerializeField] private float agilityExpGain = .01f;
    [SerializeField] private float gravity = 20.0f;

    [Header("Input Data")]
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
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        
        SetSoundType(StealthAgent.SoundType.WALK);

        if(debugView) Debug.Log("State: Entered [Ground State]");
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
            // Running
            if(inputVector != Vector3.zero && inputState.Run.State == VButtonState.PRESSED)
            {
                if(characterStats.DepleteStamina(staminaDepleteRate * Time.deltaTime))
                {
                    characterStats.IncreaseAttributeExp("agility", agilityExpGain * Time.deltaTime);
                    characterStats.DepletePotentailStamina(potentialStaminaDepleteRate * Time.deltaTime);

                    // Set relevant speed
                    //targetVelocity *= runSpeedPerAgilityLevel[characterStats.GetAttributeLevel("agility") - 1];
                    targetVelocity *= _runSpeed.GetValue(characterStats);

                    // Slide down slopes
                    if(GroundSlope > minSlideAngle) 
                    {
                        Vector3 slopeSide = Vector3.Cross(GroundNormal, Vector3.up);
                        Vector3 slopeDown = Vector3.Cross(GroundNormal, slopeSide);

                        // Make sure we are going down the slope
                        Vector3 projected = Vector3.ProjectOnPlane(targetVelocity, GroundNormal);
                        float dotProduct = Vector3.Dot(slopeDown.normalized, projected.normalized);
                        if(dotProduct > .8f)
                            targetVelocity += slopeDown * slopeSlideSpeed;
                    }

                }
                else
                    targetVelocity *= walkSpeed;
            }
            else // Walking
                targetVelocity *= walkSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            // Dash
            if(inputState.DoubleForward || inputState.DoubleBack || inputState.DoubleLeft || inputState.DoubleRight)
                if(characterStats.DepleteStamina(20))
                    _stateMachine.SwitchState(4);
        }

        // Kick
        if (inputState.Kick.State == VButtonState.PRESS_START) 
            kick.ActivateKick();

        // Switch to AirSTate
        if (IsGrounded == false) 
        {
            _stateMachine.SwitchState(5);
            return;
        }

        // Jump
        if (inputState.Jump.Pressed && IsGrounded) 
        {
            _stateMachine.SwitchState(2);
            return;
        }

        // Crouch
        if (inputState.Crouch.Pressed) 
        {
            _stateMachine.SwitchState(1);
            return;
        }
        
        // Switch to ClimbState
        if(isClimbing)
        {
            _stateMachine.SwitchState(3);
            return;
        }

        // Switch to SitState
        if (inputState.Sit.Pressed && inputState.AxisInput.magnitude == 0) 
        {
            _stateMachine.SwitchState(7);
            return;
        }

	    // We apply gravity manually for more tuning control
        if(gravityOn)
	        rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
    }

    #region Checks

    private void ClimbCheck()
    {   
        if(climbCheck == null) return;
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
