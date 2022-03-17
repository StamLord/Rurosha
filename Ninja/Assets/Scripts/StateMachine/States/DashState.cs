using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState
{
    [Header("Control Settings")]
    //[SerializeField] private float dashDistance = 3f;
    [SerializeField] private AttributeDependant<float> dashDistance;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float colliderHeight;
    [SerializeField] private new CapsuleCollider collider;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravity = 20.0f;

    [Space(20f)]

    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;

    private Vector3 dashStart;
    private Vector3 dashDirection;

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
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if(gravityOn)
            rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));

        // Stop Dash if traveled enough distance OR velocity dropped below threshold (For example if hit a wall)
        float distance = dashDistance.GetValue(characterStats);
        if((rigidbody.position - dashStart).sqrMagnitude > distance * distance || 
            rigidbody.velocity.magnitude < dashSpeed * .75f)
        {
            rigidbody.velocity = Vector3.zero;
            if(isGrounded)
                _stateMachine.SwitchState(1);
            else
                _stateMachine.SwitchState(5);

        }
    }
}
