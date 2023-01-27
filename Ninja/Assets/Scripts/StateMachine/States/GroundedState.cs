using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]

public class GroundedState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private bool gravityOn = true;
    
    [Space(20f)]
    
    [Header("Stats")]
    [SerializeField] private float gravity = 20.0f;

    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
	[SerializeField] private float maxVelocityChange = 10.0f;
    
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
        
        colliderManager.SetBody(ColliderManager.BodyCollider.STAND);
        colliderManager.SetLegs(ColliderManager.BodyCollider.STAND);
        SetSoundType(StealthAgent.SoundType.WALK);

        SetStepSoundAgent(true);

        if(debugView) Debug.Log("State: Entered [Ground State]");
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        ClimbCheck();

        // Input
        inputVector = inputState.AxisInput;
        inputVector.Normalize();
        
        targetDirection = transform.TransformDirection(inputVector);
        Vector3 targetVelocity = targetDirection;
        
        targetVelocity *= walkSpeed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        // Running
        if(inputState.Run.State == VButtonState.PRESSED)
        {
            SwitchState(CharacterStateMachine.StateName.RUN);
            return;
        }

        // Dash
        if(inputState.DoubleForward || inputState.DoubleBack || inputState.DoubleLeft || inputState.DoubleRight)
            if(characterStats.DepleteStamina(20))
                SwitchState(CharacterStateMachine.StateName.DASH);

        // Kick
        if (inputState.Kick.State == VButtonState.PRESS_START) 
            kick.ActivateKick(inputState.AxisInput);

        // Switch to AirState
        if (IsGrounded == false) 
        {
            SwitchState(CharacterStateMachine.StateName.AIR);
            return;
        }

        // Jump
        if (inputState.Jump.Pressed && IsGrounded) 
        {
            SwitchState(CharacterStateMachine.StateName.JUMP);
            return;
        }

        // Crouch
        if (inputState.Crouch.Pressed) 
        {
            SwitchState(CharacterStateMachine.StateName.CROUCH);
            return;
        }
        
        // Switch to ClimbState
        if(isClimbing)
        {
            SwitchState(CharacterStateMachine.StateName.CLIMB);
            return;
        }

        // Switch to SitState
        if (inputState.Sit.Pressed && inputState.AxisInput.magnitude == 0) 
        {
            SwitchState(CharacterStateMachine.StateName.SIT);
            return;
        }

	    // We apply gravity manually for more tuning control
        if(gravityOn)
	        rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
    }

    protected override void OnExitState()
    {
        SetStepSoundAgent(false);
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
