using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private float rollDistance = 3f;
    [SerializeField] private float rollSpeed = 10f;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravity = 20.0f;

    [Space(20f)]

    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;

    private Vector3 rollStart;
    private Vector3 rollDirection;
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
        if(debugView) Debug.Log("State: Entered [Roll State]");

        colliderManager.SetBody(ColliderManager.BodyCollider.CROUCH);
        colliderManager.SetLegs(ColliderManager.BodyCollider.CROUCH);

        rollStart = rigidbody.position;
        startTime = Time.time;
        
        // Roll is always forward
        // Sum of forward and up is projected on a plane with world up as normal
        rollDirection = Vector3.ProjectOnPlane(transform.up + transform.forward, Vector3.up);

        // Set rigidbody speed
        rigidbody.velocity = rollDirection * rollSpeed;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if(gravityOn)
            rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
        Debug.Log("Rolling rolling");
        // Stop roll if traveled enough distance, velocity drops below threshold (For example if hit a wall)
        // OR if enough time has passed (Edge case if we get stuck with velocity not dropping)
        if((rigidbody.position - rollStart).sqrMagnitude > rollDistance * rollDistance || 
            rigidbody.velocity.magnitude < rollSpeed * .75f ||
            Time.time - startTime >= rollDistance / rollSpeed)
        {
            rigidbody.velocity = Vector3.zero;
            if(IsGrounded) // CrouchState
                _stateMachine.SwitchState(1);
            else // AirState
                _stateMachine.SwitchState(5);

        }
    }
}
