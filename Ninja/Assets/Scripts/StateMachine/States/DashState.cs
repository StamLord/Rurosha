using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private AttributeDependant<float> dashDistance;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private Material material;
    [SerializeField] private float screenFXMult = 0.02f;

    [Space(20f)]

    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;

    private Vector3 dashStart;
    private Vector3 dashDirection;
    private float distance;
    private float startTime;

    void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Dash State]");

        dashStart = rigidbody.position;
        Vector3 inputVector = inputState.AxisInput;
        inputVector.Normalize();
        
        //Safeguard, default is dash forward
        if(inputVector.sqrMagnitude == 0)
            dashDirection = transform.forward;
        else
            dashDirection = transform.TransformDirection(inputVector);

        rigidbody.velocity = dashDirection * dashSpeed;
        distance = dashDistance.GetValue(characterStats);
        startTime = Time.time;

        // Set screen effect
        if(material)
            material.SetFloat("_strength", screenFXMult);
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if(gravityOn)
            rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));

        // Stop Dash if traveled enough distance, velocity drops below threshold (For example if hit a wall)
        // OR if enough time has passed (Edge case if we get stuck with velocity not dropping)
        if((rigidbody.position - dashStart).sqrMagnitude > distance * distance || 
            rigidbody.velocity.magnitude < dashSpeed * .75f ||
            Time.time - startTime >= distance / dashSpeed)
        {
            rigidbody.velocity = Vector3.zero;
            if(IsGrounded)
                _stateMachine.SwitchState(1);
            else
                _stateMachine.SwitchState(5);

        }
    }

    protected override void OnExitState()
    {
        // Set screen effect
        if(material)
            material.SetFloat("_strength", 0f);
    }

}
