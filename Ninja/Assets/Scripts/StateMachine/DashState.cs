using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : State
{
    [Header("Control Settings")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float colliderHeight;
    [SerializeField] private new CapsuleCollider collider;

    [Space(20f)]

    [Header("Stats")]
    [SerializeField] private CharacterStats characterStats;

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
        characterStats = ((PlayerControls)_stateMachine).characterStats;

        dashStart = rigidbody.position;
        Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputVector.Normalize();
        dashDirection = transform.TransformDirection(inputVector);

        collider.height = colliderHeight;

        rigidbody.velocity = dashDirection * dashSpeed;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if((rigidbody.position - dashStart).sqrMagnitude > dashDistance * dashDistance)
            _stateMachine.SwitchState(1);
    }
}
