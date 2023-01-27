using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private float climbSpeed = 4.0f;
    [SerializeField] private float climbSpeedHorizontal = 4.0f;
	[SerializeField] private float maxVelocityChange = 10.0f;

    [Space(20f)]

    [Header("Stamina Cost")]
    [SerializeField] private float staticCost = 2f;
    [SerializeField] private float dynamicCost = 5f;

    [Space(20f)]

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

    [Header("Ledge Detection")]
    [SerializeField] private float ledgeOffset = .1f;
    [SerializeField] private bool ledgeDetected;
    [SerializeField] private Vector3 ledgePoint;

    [Space(20f)]

    [Header("Ledge Transitions")]
    [SerializeField] private float ledgeClimbTime = 1f;
    [SerializeField] private float ledgeTransTimer;
    [SerializeField] private bool inTransition;

    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private Color ledgeColor = Color.red;
    [SerializeField] private Color climbCheckColor = Color.yellow;
    [SerializeField]private new Rigidbody rigidbody;

    void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Climb State]");

        rigidbody.isKinematic = true;
        SetSoundType(StealthAgent.SoundType.NONE);
        inputState = ((CharacterStateMachine)_stateMachine).inputState;
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if(inTransition) return;
        
        GetInput();

        // Stamina deplete
        float staminaCost = staticCost;

        if(inputVector.magnitude > 0)
            staminaCost = dynamicCost;

        if(characterStats.DepleteStamina(staminaCost * Time.deltaTime, true) == false)
        {
            // Fall if not enough Stamina
            SwitchState(CharacterStateMachine.StateName.AIR);
            return;
        }


        ClimbCheck();
        GetWallAxis();
        DetectLedge();
        
        Vector3 moveVector = wallVertical * inputVector.y + wallHorizontal * inputVector.x;
        moveVector.Normalize();

        // Check if we have wall to move to
        RaycastHit wallToClimb;
        Vector3 projection = moveVector;
        projection.y *= 15f;

        Debug.DrawRay(climbCheck.position + (projection * Time.deltaTime), -wallNormal * 1f);
        if(Physics.Raycast(climbCheck.position + (projection * Time.deltaTime), -wallNormal, out wallToClimb, 1f ,climbMask))
        {
            moveVector *= climbSpeed;
            Debug.Log(wallToClimb.transform.gameObject);
            RaycastHit obstacle;
            if(Physics.Raycast(transform.position, moveVector, out obstacle, 1f, blockMask) == false)
                rigidbody.MovePosition(transform.position + (moveVector * Time.deltaTime));
        }
        else
        {
            // Moving up and there is a ledge
            if(moveVector.y > 0 && ledgeDetected)
            {   
                // Start Ledge transition
                IEnumerator co = LedgeTransition(ledgePoint);
                StartCoroutine(co);
            }
        }

        #region Transitions
            
        if(ClimbDownCheck())
            SwitchState(CharacterStateMachine.StateName.WALK);
        
        // Climb Down
        if(moveVector.y < 0)
        {
            RaycastHit floor;
            if(Physics.Raycast(transform.position, -wallVertical, out floor, 1f))
                SwitchState(CharacterStateMachine.StateName.WALK);
        }

        // Jump off
        if(inputState.Jump.State == VButtonState.PRESS_START)
            SwitchState(CharacterStateMachine.StateName.JUMP);

        // Fall if no longer climbing
        if(isClimbing == false)
            SwitchState(CharacterStateMachine.StateName.AIR);

        #endregion
    }

    protected override void OnExitState()
    {
        base.OnExitState();

        rigidbody.isKinematic = false;
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
        if(Physics.Raycast(climbCheck.position, transform.forward, out wall, 1f, climbMask))
        {    
            wallPoint = wall.point;
            wallNormal = wall.normal;
            CalculateWallAxis();
            climbingOn = wall.transform;
        }
        else
            climbingOn = null;
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
        SwitchState(CharacterStateMachine.StateName.WALK);        
    }

    private void DetectLedge()
    {
        RaycastHit ledge;
        Ray ledgeRay = new Ray(wallPoint - wallNormal * ledgeOffset + Vector3.up - Vector3.up * .1f, Vector3.down);
        
        ledgeDetected = Physics.Raycast(ledgeRay, out ledge, 1f);
        ledgePoint = ledge.point;

        Debug.DrawRay(wallPoint - wallNormal * ledgeOffset + Vector3.up - Vector3.up * .1f, Vector3.down, Color.cyan);
    }

    private bool ClimbDownCheck()
    {
        if(inputVector.y > 0) return false;
        return IsGrounded;
    }

    private void OnDrawGizmos() 
    {
        if(debugView == false) return;

        Gizmos.color = ledgeColor;
        Gizmos.DrawCube(ledgePoint, new Vector3(.1f, .1f, .1f));

        Gizmos.color = climbCheckColor;
        Gizmos.DrawWireSphere(climbCheck.position, 2f);
    }
}
