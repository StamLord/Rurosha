using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveState : MonoBehaviour
{
    public InputState inputState;
    public Vector3 inputVector;
    public Vector3 targetDirection;

    public float moveSpeed = 1;
    public float runSpeed = 5;
    public float maxVelocityChange = 10;

    public Rigidbody rigidbody;
    public Animator animator;

    void Update()
    {
        // Input
        inputVector = inputState.AxisInput;
        targetDirection = inputVector;
        Vector3 targetVelocity = targetDirection;

        if(inputState.run.State == VButtonState.PRESSED)
            targetVelocity *= runSpeed;
        else
            targetVelocity *= moveSpeed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        if(targetDirection.magnitude != 0)
        {
            Quaternion changeInRotation = Quaternion.FromToRotation(Vector3.forward, targetDirection);
            Vector3 euler = changeInRotation.eulerAngles;
            // transform.RotateAround(transform.position, Vector3.up, inputState.rotation * Time.deltaTime);

            rigidbody.MoveRotation(Quaternion.Euler(Vector3.Lerp(rigidbody.rotation.eulerAngles, euler, .05f)));
        }

        animator.SetFloat("Speed", rigidbody.velocity.magnitude);
    }
}
