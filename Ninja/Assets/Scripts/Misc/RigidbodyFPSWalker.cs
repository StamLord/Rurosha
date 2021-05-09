using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
 
public class RigidbodyFPSWalker : MonoBehaviour {
 
 public struct JumpState
 {
    public JumpState(float timeStamp, Vector3 targetDirection, bool isGrounded)
    {
        _timeStamp = timeStamp;
        _targetDirection = targetDirection;
        _isGrounded = isGrounded;
    }

    public float _timeStamp { get; }
    public Vector3 _targetDirection { get; }
    public bool _isGrounded{ get; }

 }

    [Header("Control Settings")]
	[SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private float runSpeed = 15.0f;
    [SerializeField] private float airControl = 5f;
    [SerializeField] private bool gravityOn = true;

    [SerializeField] private bool isCrouching;
    [SerializeField] private Vector3 standingColliderSize;
    [SerializeField] private Vector3 crouchingColliderSize;
    [SerializeField] private new CapsuleCollider collider;
    
    [Space(20f)]
	
    [SerializeField] private float gravity = 10.0f;
	[SerializeField] private float maxVelocityChange = 10.0f;
	
    [Space(20f)]
    
    [SerializeField] private float jumpHeight = 2.0f;
    
    [Space(20f)]
    
    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private JumpState lastJumpState;
    
    [Space(20f)]
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundSlope;
    [SerializeField] private float groundSphereRadius = .4f;
    [SerializeField] private float groundDistance = .4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGrounded;

    [Space(20f)]
    
    [Header("Ground Detection")]
    [SerializeField] private Transform ceilCheck;
    [SerializeField] private float ceilDistance = .5f;
    
    [Space(20f)]

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpWindow= .2f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private bool wallJumpOn = true;
    [SerializeField] private Transform wallJumpCheck;
    [SerializeField] private float wallJumpDistance = .7f;
    [SerializeField] private float wallJumpMinimumSlope = -.1f;
    [SerializeField] private LayerMask wallJumpMask;
    [SerializeField] private GameObject wallCollided;
    [SerializeField] private Vector3 wallHitNormal;

    [Space(20f)]

    [Header("Wall Running")]
    [SerializeField] private bool wallRunOn = true;
    [SerializeField] private bool isWallRunning;
    [SerializeField] private float wallRunDistance = .7f;
    [SerializeField] private float wallRunMinimumSlope = -.1f;
    [SerializeField] private LayerMask wallRunMask;
    [SerializeField] private Transform wallRunCheck;
    [SerializeField] private Vector3 wallRunCheckVector = Vector3.right;
    [SerializeField] private Vector3 wallRunNormal;
    [SerializeField] private GameObject runningOnWall;
    [SerializeField] private Vector3 vectorAlongWall;
    
    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField]private new Rigidbody rigidbody;

    public bool Crouching {get { return isCrouching;} }
    public Vector3 Speed {get; private set;}

	void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity = false;

        collider = GetComponent<CapsuleCollider>();
	}
    
	void FixedUpdate () 
    {
        GroundCheck();

        // Input
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputVector.Normalize();
        targetDirection = transform.TransformDirection(inputVector);
        Vector3 targetVelocity = targetDirection;

        // Wall Run
        if (isWallRunning)
        {
            WallRunCheck();
            if(runningOnWall)
            {
                targetVelocity = vectorAlongWall * walkSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rigidbody.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            else
                StopWallRunning();
        }
        // Ground Control
	    else if (isGrounded) 
        {
            targetVelocity *= (Input.GetButton("Run")) ? runSpeed : walkSpeed;
                        
            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        // Air Control
        else 
        {
            rigidbody.AddForce(targetVelocity * airControl, ForceMode.Acceleration);
        }

        // Jump
        if(Input.GetButtonDown("Jump"))
        {
            lastJumpState = new JumpState (Time.time, targetDirection, isGrounded);

            // WallRunCheck();
            
            // if (wallRunOn && runningOnWall)
            //     StartWallRunning();
            
        }
        else if (Input.GetButton("Jump")) 
        {
            if(isGrounded && isWallRunning == false)
            {
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
            }
            
        }

        // Check if wall jumped
        if (wallJumpOn)
        {
            WallJumpCheck();
            if(wallCollided != null && wallHitNormal.y >= wallJumpMinimumSlope)
            {
                rigidbody.velocity = wallHitNormal * wallJumpForce;
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, CalculateJumpVerticalSpeed(), rigidbody.velocity.z);
                wallCollided = null;
            }
        }

        // Crouch
        if (Input.GetKey(KeyCode.C)) 
            Crouch();
        else 
            Uncrouch();

        

	    // We apply gravity manually for more tuning control
        if(gravityOn)
	        rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
 
	    //isGrounded = false;

        Speed = transform.InverseTransformVector(rigidbody.velocity) / runSpeed;
	}
 
	private void GroundCheck()
    {
        RaycastHit groundHit;
        isGrounded = Physics.SphereCast(groundCheck.position, groundSphereRadius, Vector3.down, out groundHit, groundDistance, groundMask);
        groundSlope = Vector3.Angle(groundHit.normal, Vector3.up);
    }

    private void OnDrawGizmos()
    {
        if(!debugView) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, groundSphereRadius);
        Gizmos.DrawWireSphere(groundCheck.position + Vector3.down * groundDistance, groundSphereRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallJumpCheck.position, wallJumpDistance);

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(wallRunCheck.position, -wallRunCheckVector);
        Gizmos.DrawLine(wallRunCheck.position, wallRunCheckVector);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, targetDirection);
    }

    private void WallJumpCheck()
    {
        wallCollided = null;

        if(Time.time - lastJumpState._timeStamp > wallJumpWindow || lastJumpState._isGrounded) return;
        
        RaycastHit wall;

        if(Physics.Raycast(wallJumpCheck.position, lastJumpState._targetDirection, out wall, wallJumpDistance, wallJumpMask) /*||
            Physics.Raycast(wallJumpCheck.position, rigidbody.velocity, out wall, wallJumpDistance, wallJumpMask)*/)
        {
            wallCollided = wall.transform.gameObject;
            wallHitNormal = wall.normal;
        }
    }

    private void WallRunCheck()
    {
        runningOnWall = null;

        RaycastHit leftWall;
        RaycastHit rightWall;

        Vector3 leftCheck = transform.TransformDirection(-wallRunCheckVector);
        Vector3 rightCheck = transform.TransformDirection(wallRunCheckVector);

        bool isLeft = Physics.Raycast(wallRunCheck.position, leftCheck, out leftWall, wallRunDistance, wallRunMask);
        bool isRight = Physics.Raycast(wallRunCheck.position, rightCheck, out rightWall, wallRunDistance, wallRunMask);

        if(isLeft && isRight)
        {
            if(Vector3.Distance(leftWall.point, transform.position) > Vector3.Distance(rightWall.point, transform.position))
            {
                wallRunNormal = rightWall.normal;
                runningOnWall = rightWall.transform.gameObject;
            }
            else
            {
                wallRunNormal = leftWall.normal;
                runningOnWall = leftWall.transform.gameObject;
            }
        }
        else if(isLeft)
        {
            wallRunNormal = leftWall.normal;
            runningOnWall = leftWall.transform.gameObject;
        }
        else if(isRight)
        {
            wallRunNormal = rightWall.normal;
            runningOnWall = rightWall.transform.gameObject;
        }

        Debug.Log(runningOnWall);
    }

    private void Crouch()
    {
        if(isCrouching) return;
        
        isCrouching =true;
        collider.height = crouchingColliderSize.y;
    }

    private void Uncrouch()
    {
        if(Physics.Raycast(ceilCheck.position, transform.up, ceilDistance))
            return;

        isCrouching = false;
        collider.height = standingColliderSize.y;
    }
 
	float CalculateJumpVerticalSpeed () 
    {
	    // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
	    return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

    private void StartWallRunning()
    {
        isWallRunning = true;
        gravityOn = false;
        Vector3 moveDirection = transform.TransformDirection(new Vector3(0,0,inputVector.z).normalized);
        vectorAlongWall = Vector3.ProjectOnPlane(moveDirection, wallRunNormal).normalized;

        //Debug.Break();
        Debug.DrawRay(transform.position, moveDirection * 2f, Color.green);
        Debug.DrawRay(transform.position, wallRunNormal, Color.red);
        Debug.DrawRay(transform.position, vectorAlongWall, Color.black);
    }

    private void StopWallRunning()
    {
        isWallRunning = false;
        gravityOn = true;
    }
}