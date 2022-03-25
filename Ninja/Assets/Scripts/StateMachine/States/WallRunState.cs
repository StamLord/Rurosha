using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunState : PlayerState
{
     [Header("Control Settings")]
    [SerializeField] private AttributeDependant<float> runDistance;
    [SerializeField] private float runSpeed = 20f;

    [Space(20f)]

    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;

    private Vector3 runStart;
    private Vector3 runDirection;
    public bool wallOnLeft;

    public delegate void RunStartDelegate(bool wallOnLeft);
    public event RunStartDelegate OnRunStart;

    public delegate void RunEndDelegate();
    public event RunEndDelegate OnRunEnd;

    void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [WallRun State]");

        runStart = rigidbody.position;
        runDirection = Vector3.Cross(transform.up, wallNormal);

        wallOnLeft = (Vector3.Dot(runDirection, transform.forward) <= 0);
        if(wallOnLeft)
            runDirection = -runDirection;

        rigidbody.velocity = runDirection * runSpeed;

        if(OnRunStart != null)
            OnRunStart(wallOnLeft);
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        // Stop run if traveled enough distance OR velocity dropped below threshold (For example if hit a wall)
        float distance = runDistance.GetValue(characterStats);
        
        bool reachedMaxDist = (rigidbody.position - runStart).sqrMagnitude > distance * distance;
        bool velocityDropped =  rigidbody.velocity.magnitude < runSpeed * .75f;
        Vector3 point, normal;
        float angle;

        bool stillOnWall = (wallOnLeft) ? WallDetect(-transform.right, out point, out normal, out angle) : WallDetect(transform.right, out point, out normal, out angle);

        if(reachedMaxDist|| velocityDropped || stillOnWall == false)
        {
            if(isGrounded)
                _stateMachine.SwitchState(1);
            else
                _stateMachine.SwitchState(5);

        }
    }

    protected override void OnExitState()
    {
        base.OnExitState();
        if(OnRunEnd != null)
            OnRunEnd();
    }
}
