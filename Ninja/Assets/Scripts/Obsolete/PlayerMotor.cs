using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;

    private float verticalVelocity;
    public float gravity = 14f;
    public float jumpForce = 10f;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float wallJumpForce = 2f;
    public float airControlPrecentage = 2f;
    
    private Vector3 lastMove;
    private Vector3 moveVector;

    public bool jumpStarted;
    public float jumpBar;
    public float jumpBarFillRate = .5f;
    public float minimumJumpPrecentage = .65f;

    public bool wallStickStarted;
    public float wallStickDuration = .5f;
    public float wallStickStartTime;
    public ControllerColliderHit wallStickingTo;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.z = Input.GetAxis("Vertical");
        moveVector.y = 0;

        if(controller.isGrounded == false && wallStickStarted == false) {
            moveVector = moveVector.normalized * .5f;
            moveVector += lastMove.normalized;
            moveVector.Normalize();
            moveVector *= (Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed;
        }
        else{
            moveVector.Normalize();
            moveVector *= (Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed;
        }

        // Apply Gravity - Unless sticking to a wall (Holding Space)
        if(controller.isGrounded == false && wallStickStarted == false) {
            verticalVelocity -= gravity * Time.deltaTime;            
        }

        if(controller.isGrounded){
            verticalVelocity = -gravity * Time.deltaTime;

            if(wallStickStarted)
                WallJumpRelease();

            if(Input.GetButtonDown("Jump"))
                JumpStart();
            else if(Input.GetButtonUp("Jump"))
            {
                verticalVelocity = jumpForce * (minimumJumpPrecentage + ((1f - minimumJumpPrecentage) * jumpBar));
                JumpRelease();
            }
            else if (jumpStarted)
                JumpBarFill();
        }
        else if(wallStickingTo != null)
        {
            if(Input.GetButtonDown("Jump"))
            {   
                WallJumpStart();
                moveVector = -controller.velocity;
            }
            else if(Input.GetButtonUp("Jump"))
            {
                //verticalVelocity = wallJumpForce;
                moveVector = wallStickingTo.normal * wallJumpForce;
                Debug.DrawRay(wallStickingTo.point, wallStickingTo.normal * wallJumpForce, Color.red);
                // Debug.Break();
                Debug.Log(wallStickingTo.normal);
                WallJumpRelease();
            }
        }

        moveVector.y = verticalVelocity;
        
        controller.Move(moveVector * Time.deltaTime);
        lastMove = moveVector;
    }

    private void JumpStart()
    {
        Debug.Log("Jump Start");
        WallJumpRelease();
        jumpStarted = true;
    }

    private void JumpRelease()
    {
        Debug.Log("Jump Release");
        jumpStarted = false;
        jumpBar = 0f;
    }

    private void JumpBarFill()
    {
        jumpBar = Mathf.Clamp(jumpBar + jumpBarFillRate * Time.deltaTime, 0, 1);
    }

    private void WallJumpStart()
    {
        Debug.Log("WallJump Start");
        JumpRelease();
        wallStickStarted = true;
        wallStickStartTime = Time.deltaTime;
    }

    private void WallJumpRelease()
    {
        Debug.Log("WallJump Release");
        wallStickStarted = false;
        
        wallStickingTo = null;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(!controller.isGrounded && hit.normal.y < 0.1f){
            // if(Input.GetButtonUp("Jump"))
            // {
            //     // Debug.DrawRay(hit.point, hit.normal, Color.red);
            //     verticalVelocity = wallJumpForce;
            //     moveVector = hit.normal * wallJumpForce;
            // }
            // else if (Input.GetButtonDown("Jump"))
            // {
                wallStickingTo = hit;
                
            // }
        }
    }

}
