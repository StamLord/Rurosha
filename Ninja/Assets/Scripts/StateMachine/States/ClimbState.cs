using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbState : State
{
    [Header("Control Settings")]
    [SerializeField] private float climbSpeed = 4.0f;
    [SerializeField] private Vector3 climbingColliderSize;
    [SerializeField] private new CapsuleCollider collider;
    [Space(20f)]
	
	[SerializeField] private float maxVelocityChange = 10.0f;

    [Header("Input Data")]
    [SerializeField] private InputState inputState;
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private Vector3 targetVelocity;
    
    [Space(20f)]
    
    [Header("Climb Detection")]
    [SerializeField] private Transform climbCheck;
    [SerializeField] private Transform climbingOn;
    [SerializeField] private LayerMask climbMask;
    [SerializeField] private LayerMask blockMask;
    [SerializeField] private bool isClimbing;
    [SerializeField] private Vector3 wallPoint;
    [SerializeField] private Vector3 wallNormal;
    [SerializeField] private Vector3 wallVertical;
    [SerializeField] private Vector3 wallHorizontal;

    [Space(20f)]

    [Header("Climb Transitions")]
    [SerializeField] private float ledgeClimbTime = 1f;
    [SerializeField] private float ledgeTransTimer;
    [SerializeField] private bool inTransition;

    [Space(20f)]

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundSlope;
    [SerializeField] private float groundSphereRadius = .4f;
    [SerializeField] private float groundDistance = .8f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGrounded;

    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField]private new Rigidbody rigidbody;

    [SerializeField]private PhysicMaterial noFriction;
    [SerializeField]private PhysicMaterial friction;

    void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;

        collider = GetComponent<CapsuleCollider>();
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Climb State]");
        collider.height = climbingColliderSize.y;

        collider.material = friction;
        rigidbody.isKinematic = true;

        inputState = ((CharacterStateMachine)_stateMachine).inputState;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if(inTransition) return;
        
        GetInput();

        #region Old Physics Movement
        // targetVelocity *= climbSpeed;

        // Vector3 velocity = rigidbody.velocity;
        // Vector3 velocityChange = (targetVelocity - velocity);

        // velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        // velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
        // velocityChange.z = 0;

        //rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        #endregion

        ClimbCheck();
        GetWallAxis();
        
        Vector3 moveVector = wallVertical * inputVector.y + wallHorizontal * inputVector.x;
        moveVector.Normalize();

        // Check if we have wall to move to
        RaycastHit wallToClimb;
        if(Physics.Raycast(transform.position + (moveVector * Time.deltaTime), -wallNormal, out wallToClimb, climbMask))
        {
            moveVector *= climbSpeed;

            RaycastHit obstacle;
            if(Physics.Raycast(transform.position, moveVector, out obstacle, 1f, blockMask) == false)
                rigidbody.MovePosition(transform.position + (moveVector * Time.deltaTime));
        }
        else
        {
            // Moving up along the wall
            if(moveVector.y > 0)
            {   
                RaycastHit ledge;
                Debug.DrawRay(wallPoint - wallNormal + Vector3.up, Vector3.down, Color.cyan);
                if(Physics.Raycast(wallPoint - wallNormal + Vector3.up, Vector3.down, out ledge, 1f))
                {
                    // Start Ledge transition
                    IEnumerator co = LedgeTransition(ledge.point);
                    StartCoroutine(co);
                }
            }
        }
        
        // if(climbingOn)
        // {
        //     // Rotate Player
        //     Vector3 facingVector = climbingOn.position - transform.position;
        //     facingVector.y = 0;
        //     transform.forward = facingVector;
        // }

        #region Transitions
            
        if(ClimbDownCheck())
            _stateMachine.SwitchState(0);
        
        // Climb Down
        if(moveVector.y < 0)
        {
            RaycastHit floor;
            if(Physics.Raycast(transform.position, -wallVertical, out floor, 1f))
                _stateMachine.SwitchState(1);
        }


        // Jump off
        if(inputState.Jump.State == VButtonState.PRESS_START)
            _stateMachine.SwitchState(2);

        // Fall if no longer climbing
        if(isClimbing == false)
            _stateMachine.SwitchState(2);

        #endregion
    }

    protected override void OnExitState()
    {
        base.OnExitState();

        rigidbody.isKinematic = false;
        collider.material = noFriction;
    }

    private void GetInput()
    {
        inputVector = new Vector3(inputState.AxisInput.x, inputState.AxisInput.z, 0);
        inputVector.Normalize();
        targetDirection = transform.TransformDirection(inputVector);
        targetVelocity = targetDirection;
    }

    private void ClimbCheck()
    {
        // We are climbing if close enough to any "Climable" layer colliders
        Collider[] colliders = Physics.OverlapSphere(climbCheck.position, 2f, climbMask);
        isClimbing = (colliders.Length > 0);
    }

    private void GetWallAxis()
    {
        // Get wall and normal infront
        RaycastHit wall;
        if(Physics.Raycast(transform.position, transform.forward, out wall, 1f, climbMask))
        {    
            wallPoint = wall.point;
            wallNormal = wall.normal;
            CalculateWallAxis();
            climbingOn = wall.transform;
        }
        else
        {
            climbingOn = null;
        }
    }

    private void CalculateWallAxis()
    {
        wallHorizontal = Vector3.Cross(wallNormal, Vector3.up);
        wallVertical = Vector3.Cross(-wallNormal, wallHorizontal);
    }

    private IEnumerator LedgeTransition(Vector3 targetPoint)
    {
        inTransition = true;
        ledgeTransTimer = 0f;
        Vector3 origin = transform.position;

        while(ledgeTransTimer < ledgeClimbTime)
        {   
            transform.position = (Vector3.Lerp(origin, targetPoint, ledgeTransTimer / ledgeClimbTime));
            ledgeTransTimer += Time.deltaTime;
            yield return null;
        }

        inTransition = false;

        // Switch to Grounded State
        _stateMachine.SwitchState(1);        
    }

    private bool ClimbDownCheck()
    {
        if(inputVector.y > 0) return false;

        RaycastHit groundHit;
        isGrounded = Physics.SphereCast(groundCheck.position, groundSphereRadius, targetDirection, out groundHit, groundDistance, groundMask);
        groundSlope = Vector3.Angle(groundHit.normal, Vector3.up);

        return isGrounded;
    }
}
