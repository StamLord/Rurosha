using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]

public class RunState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private AttributeDependant<float> runSpeed;
    [SerializeField] private float startRunBoost = 2f;
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

        if(debugView) Debug.Log("State: Entered [Run State]");
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

        // Ground Control
        if (IsGrounded) 
        {
            // Exit Running
            if(inputVector == Vector3.zero || inputState.Run.State == VButtonState.UNPRESSED)
            {
                SwitchState(CharacterStateMachine.StateName.WALK);
                return;
            }

            // Running
            
            // Initial cost of run start
            if(inputState.Run.State == VButtonState.PRESS_START)
                characterStats.DepleteStamina(5f);

            // Deplete Stamina
            if(characterStats.DepleteStamina(staminaDepleteRate * Time.deltaTime) == false)
            {
                SwitchState(CharacterStateMachine.StateName.WALK);
                return;
            }
            
            // Deplete Potential Stamina
            characterStats.DepletePotentailStamina(potentialStaminaDepleteRate * Time.deltaTime);

            // Increase Attribute 
            characterStats.IncreaseAttributeExp(AttributeType.AGILITY, agilityExpGain * Time.deltaTime);

            // Get running speed
            float pressTime = Mathf.Clamp01(inputState.Run.PressTime);
            float mult = Mathf.Lerp(startRunBoost, 1, pressTime);

            targetVelocity *= runSpeed.GetValue(characterStats) * mult;

            // Extra speed down slopes
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
                    SwitchState(CharacterStateMachine.StateName.DASH);
        }
        else
        {
            SwitchState(CharacterStateMachine.StateName.AIR);
            return;
        }

        // Kick
        if (inputState.Kick.State == VButtonState.PRESS_START) 
            kick.ActivateKick(inputState.AxisInput);

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
