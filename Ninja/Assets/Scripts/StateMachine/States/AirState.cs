using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float airControl = 5f;
	[SerializeField] private float maxVelocityChange = 10.0f;
    [SerializeField] private Vector3 originalSpeed;

    [Space(20f)]
    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private Vector3 targetVelocity;

    [Space(20f)]

    [Header("Climb Detection")]
    [SerializeField] private Transform climbCheck;
    [SerializeField] private LayerMask climbMask;
    [SerializeField] private bool isClimbing;
    [SerializeField] private float climbingStartDistannce = .75f;

    [Space(20f)]
    [Header("Vault Settings")]
    private bool isVaulting;
    private Vector3 vaultTarget;
    private Vector3 vaultStart;
    private float vaultTimeStart;
    [SerializeField] private float vaultDuration = .5f;
    [SerializeField] private Vector3 finalPositionOffset = new Vector3(0, .5f, 0);

    public delegate void VaultStartDelegate();
    public event VaultStartDelegate OnVaultStart;

    public delegate void RollStartDelegate();
    public event RollStartDelegate OnRollStart;

    [Space(20f)]

    [Header("Fall Damage")]
    [SerializeField] private float miniFallDamageVelocity = 2f;
    [SerializeField] private int FallDamage = 10;
    [SerializeField] private AttributeDependant<float> fallDamageHeight;
    [SerializeField] private float minRollHeight = 2f;
    [SerializeField] private float rollWindow = .1f;
    private float startY;

    [Space(20f)]

    [Header("Glide")]
    [SerializeField] public bool glideOn = false;
    [SerializeField] private float glideGravityMultiplier = .2f;
    [SerializeField] private float liftForce = 5f;
    [SerializeField] private bool isGliding = false;
    [SerializeField] private Transform glideTransform;
    [SerializeField] private float maxTurnSpeed;
    [SerializeField] private AnimationCurve forwardCurve;

    [Space(20f)]
    [Header("Air Jumps")]
    [SerializeField] private int airJumps;
    [SerializeField] private int maxAirJumps = 2;

    public delegate void DoubleJumpStartDelegate();
    public event DoubleJumpStartDelegate OnDoubleJumpStart;

    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;

    private Vector3 lastWallRunNormal;

    void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Air State]");

        colliderManager.SetLegs(ColliderManager.BodyCollider.AIR);

        startY = transform.position.y;
        originalSpeed =  new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        Vector3 inputVector = inputState.AxisInput;
        inputVector.Normalize();

        SetSoundType(StealthAgent.SoundType.NONE);
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        ClimbCheck();

        // Input
        GetInput();

        // Vault
        if(isVaulting)
        {
            float percent = (Time.time - vaultTimeStart) / vaultDuration;
            transform.position = Vector3.Lerp(vaultStart, vaultTarget, percent);

            if(percent >= 1f)
            {
                isVaulting = false;
                rigidbody.isKinematic = false;
            }

            return;
        }
        else if(characterStats.IsSkillLearned("Ledge Climb") && ledgeDetected && inputState.AxisInput.z > 0) // We must be moving forward to trigger ledge climbing
        {
            isVaulting = true;
            rigidbody.velocity = Vector3.zero; // Stop jumping, falling and any other forces
            rigidbody.isKinematic = true;
            vaultTimeStart = Time.time;
            vaultStart = transform.position;
            vaultTarget = ledgePoint + finalPositionOffset;

            if (OnVaultStart != null) OnVaultStart();
            
            return;
        }

        if(isGliding)
        {
            GlideMethod2();            
        }
        else
        {
            // Accelration Method
            //rigidbody.AddForce(targetVelocity * airControl, ForceMode.VelocityChange);
            
            // Instant Method
            targetVelocity *= airControl;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rigidbody.velocity - originalSpeed;
            Vector3 velocityChange = (targetVelocity - velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            // Clamp to air control to not gain speed on each jump (due to original speed)
            velocityChange = Vector3.ClampMagnitude(velocityChange, airControl);
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            if(gravityOn)
                rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));

            // Jump if not gliding
            if (inputState.Jump.State == VButtonState.PRESS_START && isGliding == false) 
            {
                // Start wall run if: Wall detected (in input direction), at correct angle and different than last wall normal.
                // Also player needs to move forward to activate.
                if( characterStats.IsSkillLearned("Wall Run")
                    && wallDetected 
                    && wallAngle > 90 && wallAngle < 140
                    && wallNormal != lastWallRunNormal 
                    && inputState.AxisInput.z > 0)
                {
                    lastWallRunNormal = wallNormal;
                    // Reset jump button to avoid next state processing it
                    inputState.Jump.Set(VButtonState.UNPRESSED);
                    SwitchState(CharacterStateMachine.StateName.WALL_RUN);
                    return;
                }
                // Wall jump
                else if(characterStats.IsSkillLearned("Wall Jump") && wallDetected)
                {
                    SwitchState(CharacterStateMachine.StateName.JUMP);
                }
                // Perform Air Jump if below maxAirJumps
                else if(characterStats.IsSkillLearned("Double Jump") && airJumps < maxAirJumps)
                {
                    if(OnDoubleJumpStart != null) OnDoubleJumpStart();
                    airJumps++;
                    SwitchState(CharacterStateMachine.StateName.JUMP);
                }
            }
        }

        // Kick
        if (inputState.Kick.State == VButtonState.PRESS_START) 
            kick.AirKick();
        
        // Dash
        if(inputState.DoubleForward || inputState.DoubleBack || inputState.DoubleLeft || inputState.DoubleRight)
            if(characterStats.IsSkillLearned("Dash") && characterStats.DepleteStamina(20))
                SwitchState(CharacterStateMachine.StateName.DASH);

        // Switch to GroundedState
        if (IsGrounded /*&& rigidbody.velocity.y <= 0*/) 
        {
            airJumps = 0;
            lastWallRunNormal = Vector3.zero;
            
            // If timed roll (Crouch button) - No fall damage
            if(characterStats.IsSkillLearned("Roll") && RollCheck())
            {
                // Switch to RollState
                SwitchState(CharacterStateMachine.StateName.ROLL);
                if(OnRollStart != null)
                    OnRollStart();
                return;
            }

            // Deal fall damage if not gliding
            if(isGliding == false)
                DealFallDamageDistance();

            // GroundState
            SwitchState(CharacterStateMachine.StateName.WALK);
        }

        // Switch to ClimbState
        if(characterStats.IsSkillLearned("Tree Climb") && isClimbing)
            SwitchState(CharacterStateMachine.StateName.CLIMB);
    }

    [System.Obsolete]
    private void DealFallDamageVelocity()
    {
        float velocity = transform.TransformVector(rigidbody.velocity).y;
        if(velocity > miniFallDamageVelocity) return;
        
        characterStats.SubHealth(FallDamage * (Mathf.Abs(velocity + miniFallDamageVelocity)));
    }

    private bool RollCheck()
    {
        float delta = startY - transform.position.y;
        return (delta >= minRollHeight &&
            inputState.Crouch.Pressed &&
            inputState.Crouch.PressTime < rollWindow);
    }

    private void DealFallDamageDistance()
    {
        float delta = startY - transform.position.y;
        float fallHeight = fallDamageHeight.GetValue(characterStats);
        if(delta < fallHeight) return;
        int damage = Mathf.RoundToInt(FallDamage * (delta - fallHeight));
        characterStats.GetHit(null, damage * 2, damage, Vector3.up, DamageType.Blunt);
    }

    private void GetInput()
    {
        inputVector = inputState.AxisInput;
        inputVector.Normalize();
        targetDirection = transform.TransformDirection(inputVector);
        targetVelocity = targetDirection;

        // Gliding
        if(characterStats.IsSkillLearned("Glide") 
            && inputState.Crouch.State == VButtonState.PRESSED 
            && glideOn 
            && isGliding == false)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
            isGliding = true;
        }
        else if(inputState.Crouch.State == VButtonState.UNPRESSED && isGliding)
        {
            startY = transform.position.y;
            isGliding = false;
        }
    }

    #region Checks

    private void ClimbCheck()
    {
        isClimbing = (Physics.Raycast(climbCheck.position, targetDirection, climbingStartDistannce, climbMask));
    }

    #endregion

    private void GlideMethod1()
    {
        // Gravity is always pulling down
            Vector3 gravityVector = Vector3.down * gravity * rigidbody.mass;
            
            // Lift is up relative to glide transform and scales with down velocity
            Vector3 liftVector = glideTransform.up * liftForce * -rigidbody.velocity.y;

            // Get velocity on glideTransform.forward axis
            Vector3 glideForwardVelocity = Vector3.Project(rigidbody.velocity, glideTransform.forward);
            liftVector *= glideForwardVelocity.magnitude * .5f;

            // 1 - facing up, 0.5 - facing forward, 0 - facing down
            float dotProduct = Vector3.Dot(glideTransform.forward, Vector3.up) + 1; 
            dotProduct *= .5f;

            // Multiple lift by a curve (0 _/ 1 \_ 0) based on dot product
            liftVector *= forwardCurve.Evaluate(dotProduct);
            
            // Clamp speed change on glideTransform forward
            Vector3 finalVector = gravityVector + liftVector;
            Vector3 targetForwardVelocity = Vector3.ProjectOnPlane(finalVector, glideTransform.forward);
            finalVector -= targetForwardVelocity;

            Vector3 forwardVelocityChange = targetForwardVelocity - glideForwardVelocity;
            forwardVelocityChange.x = Mathf.Clamp(forwardVelocityChange.x, -maxVelocityChange, maxVelocityChange);
            forwardVelocityChange.y = Mathf.Clamp(forwardVelocityChange.y, -maxVelocityChange, maxVelocityChange);
            forwardVelocityChange.z = Mathf.Clamp(forwardVelocityChange.z, -maxVelocityChange, maxVelocityChange);

            finalVector += forwardVelocityChange;

            // Current velocity on xz axis
            Vector3 xzVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up);
            
            // Current glideTransform forward on xz axis
            Vector3 xzGlideForward = Vector3.ProjectOnPlane(glideTransform.forward, Vector3.up);

            // Clamp turn speed
            Vector3 turnVector = xzGlideForward - xzVelocity;
            turnVector.x = Mathf.Clamp(turnVector.x, -maxTurnSpeed, maxTurnSpeed);
            turnVector.y = Mathf.Clamp(turnVector.y, -maxTurnSpeed, maxTurnSpeed);
            turnVector.z = Mathf.Clamp(turnVector.z, -maxTurnSpeed, maxTurnSpeed);

            rigidbody.AddForce(finalVector + turnVector);
    }

    private void GlideMethod2()
    {
        // Gravity is always pulling down
        Vector3 gravityVector = Vector3.down * gravity * rigidbody.mass;
        
        // Lift is up relative to glide transform and scales with down velocity
        Vector3 liftVector = glideTransform.up * liftForce * -rigidbody.velocity.y;

        // 1 - facing up, 0 - facing forward, -1 - facing down
        float dotProduct = Vector3.Dot(glideTransform.forward, Vector3.up); 

        // More lift the more we face up
        // 1 - Facing up, .5f Forward, 0 down
        liftVector *= (dotProduct + 1) * .5f;

        // Forward increases if we face forward
        Vector3 forwardVector = glideTransform.forward * 10 * forwardCurve.Evaluate((dotProduct + 1) * .5f);

        // Current velocity on xz axis
        Vector3 xzVelocity = Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up);
        
        // Current glideTransform forward on xz axis
        Vector3 xzGlideForward = Vector3.ProjectOnPlane(glideTransform.forward, Vector3.up);

        // Clamp turn speed
        Vector3 turnVector = xzGlideForward - xzVelocity;
        turnVector = Vector3.ClampMagnitude(turnVector, maxTurnSpeed);

        rigidbody.AddForce(gravityVector + liftVector + forwardVector + turnVector);
    }
}
