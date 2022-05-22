using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private new Rigidbody rigidbody;

    [Space(20f)]
    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private Vector3 targetVelocity;

    void Awake ()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
	}
    
    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        // Input
        inputVector = inputState.AxisInput.normalized;

        // Transform to character facing
        targetDirection = transform.TransformDirection(inputVector);

        // // Allign input to rope
        // Vector3 ropeDir = grapplePoint - transform.position;
        // Vector3 ropeRight = Vector3.Cross(ropeDir, Vector3.up);
        // Vector3 ropeForward = Vector3.Cross(ropeDir, ropeRight);

        // targetVelocity = ropeForward * inputVector.z + ropeRight * inputVector.x;

        targetVelocity = targetDirection * speed;
        Debug.Log(targetVelocity);
        
        // Move Rigidbody
        rigidbody.AddForce(targetVelocity, ForceMode.Acceleration);

        // Gravity
        rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
    }
}
