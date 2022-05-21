using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private float colliderHeight;
    [SerializeField] private new CapsuleCollider collider;
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

    [Space(20f)]

    [Header("Fall Damage")]
    [SerializeField] private float miniFallDamageVelocity = 2f;
    [SerializeField] private int FallDamage = 10;
    [SerializeField] private float minimumFallDamageDistance = 10;
    private float startY;

    [Space(20f)]

    [Header("Glide")]
    [SerializeField] public bool glideOn = false;
    [SerializeField] private float glideGravityMultiplier = .2f;
    [SerializeField] private bool isGliding = false;

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
        collider = GetComponent<CapsuleCollider>();
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Air State]");

        startY = transform.position.y;
        originalSpeed =  new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        Vector3 inputVector = inputState.AxisInput;
        inputVector.Normalize();
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
        else if(ledgeDetected && inputState.AxisInput.z > 0) // We must be moving forward to trigger ledge climbing
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
        {
            if(isGliding)
                rigidbody.AddForce(new Vector3 (0, -gravity * glideGravityMultiplier * rigidbody.mass, 0));
            else
                rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
        }

        // Jump if not gliding
        if (inputState.Jump.State == VButtonState.PRESS_START && isGliding == false) 
        {
            // Start wall run if: Wall detected (in input direction), at correct angle and different than last wall normal.
            // Also player needs to move forward to activate.
            if(wallDetected 
                && wallAngle > 90 && wallAngle < 140
                && wallNormal != lastWallRunNormal 
                && inputState.AxisInput.z > 0)
            {
                lastWallRunNormal = wallNormal;
                // Reset jump button to avoid next state processing it
                inputState.Jump.Set(VButtonState.UNPRESSED);
                _stateMachine.SwitchState(6);
                return;
            }
            // Wall jump
            else if(wallDetected)
            {
                _stateMachine.SwitchState(2);
            }
            // Perform Air Jump if below maxAirJumps
            else if(airJumps < maxAirJumps)
            {
                if(OnDoubleJumpStart != null) OnDoubleJumpStart();
                airJumps++;
                _stateMachine.SwitchState(2);
            }
        }

        // Dash
        if(inputState.DoubleForward || inputState.DoubleBack || inputState.DoubleLeft || inputState.DoubleRight)
            if(characterStats.DepleteStamina(20))
                _stateMachine.SwitchState(4);

        // Switch to GroundedState
        if (IsGrounded /*&& rigidbody.velocity.y <= 0*/) 
        {
            airJumps = 0;
            lastWallRunNormal = Vector3.zero;
            DealFallDamageDistance();
            _stateMachine.SwitchState(0);
        }

        // Switch to ClimbState
        if(isClimbing)
            _stateMachine.SwitchState(3);
    }

    private void DealFallDamageVelocity()
    {
        float velocity = transform.TransformVector(rigidbody.velocity).y;
        if(velocity > miniFallDamageVelocity) return;
        
        characterStats.SubHealth(FallDamage * (Mathf.Abs(velocity + miniFallDamageVelocity)));
    }

    private void DealFallDamageDistance()
    {
        float delta = startY - transform.position.y;
        if(delta < minimumFallDamageDistance) return;
        int damage = Mathf.RoundToInt(FallDamage * (delta - minimumFallDamageDistance));
        characterStats.GetHit(damage * 2, damage, DamageType.Blunt, Direction9.CENTER);
    }

    private void GetInput()
    {
        inputVector = inputState.AxisInput;
        inputVector.Normalize();
        targetDirection = transform.TransformDirection(inputVector);
        targetVelocity = targetDirection;

        if(inputState.Crouch.State == VButtonState.PRESSED && glideOn && isGliding == false)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
            isGliding = true;
        }
        else if(inputState.Crouch.State == VButtonState.UNPRESSED && isGliding)
            isGliding = false;
    }

    #region Checks

    private void ClimbCheck()
    {
        isClimbing = (Physics.Raycast(climbCheck.position, targetDirection, climbingStartDistannce, climbMask));
    }

    #endregion
}
