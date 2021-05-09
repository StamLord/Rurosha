using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    
    private float standingHeight;
    public float crouchHeight = 1f;

    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundMask;
    public bool isGrounded;

    public bool wallJumpOn = true;
    public Transform wallJumpCheck;
    public float wallJumpDistance = .7f;
    public float wallMinimumSlope = -.1f;
    public LayerMask wallMask;
    public GameObject wallCollided;
    private Vector3 wallHitNormal;
    //public LayerMask wallMask;

    public Vector3 moveVector;

    public float gravity = 14f;
    public float jumpForce = 12f;
    public float jumpStart;
    public float jumpGracePeriod = .2f;
    public float walkSpeed = 10f;
    public float runSpeed = 15;
    public float wallJumpForce = 10f;
    public float airControl = 5f;

    public Vector3 velocity;

    public bool debugView;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        standingHeight = controller.height;
    }

    void Update()
    {
        GroundCheck();

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveVector = transform.right * x + transform.forward * z;
        moveVector.Normalize();
        
        if(isGrounded) { // Ground Movement
            moveVector *= (Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed;

            if(Time.time > jumpStart + jumpGracePeriod) {
                velocity = Vector3.zero; // Reset any velocity like from wall jump
                velocity.y = -2f;
            }
        }
        else { // Air control
             moveVector *= airControl;
             
             if(moveVector.x < 0 && moveVector.x < velocity.x ||
                moveVector.x > 0 && moveVector.x > velocity.x )
                velocity.x = moveVector.x;

            velocity.y -= gravity * Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump"))
        {
            if(isGrounded){
                velocity = moveVector;
                velocity.y = jumpForce;
                jumpStart = Time.time;
            }
            else if(wallJumpOn){
                WallJumpCheck();
                if(wallCollided != null && wallHitNormal.y >= wallMinimumSlope)
                {
                    velocity = wallHitNormal * wallJumpForce;
                    velocity.y = wallJumpForce;
                    wallCollided = null;
                }
            }
        }

        controller.Move(moveVector * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.C))
            controller.height = crouchHeight;
        else if(Input.GetKeyUp(KeyCode.C))
            controller.height = standingHeight;
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void OnDrawGizmos()
    {
        if(!debugView) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallJumpCheck.position, wallJumpDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, moveVector);
    }

    private void WallJumpCheck()
    {
        RaycastHit wall;

        if(Physics.Raycast(wallJumpCheck.position, moveVector, out wall, wallJumpDistance, wallMask) ||
            Physics.Raycast(wallJumpCheck.position, velocity, out wall, wallJumpDistance, wallMask))
        {
            Debug.DrawRay(wallJumpCheck.position, moveVector, Color.yellow, 1f);
            wallCollided = wall.transform.gameObject;
            wallHitNormal = wall.normal;
        }
        else
            wallCollided = null;
    }

}
