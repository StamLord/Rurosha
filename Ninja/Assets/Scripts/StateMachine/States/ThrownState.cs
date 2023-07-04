using UnityEngine;

public class ThrownState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float stateDuration = 1f;

    [Space(20f)]
    [Header("Input Data")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 targetDirection;
    [SerializeField] private Vector3 targetVelocity;

    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;

    private Vector3 throwForce;
    private float startTime;

    private void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Thrown State]");

        colliderManager.SetLegs(ColliderManager.BodyCollider.AIR);
        
        startTime = Time.time;
        throwForce = CharacterStateMachine.ThrowForce;
        
        // Throw force
        rigidbody.velocity = Vector3.zero; // Reset existing velocity
        rigidbody.AddForce(throwForce, ForceMode.Impulse);

        SetSoundType(StealthAgent.SoundType.NONE);
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if(gravityOn)
            rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));

        // Exit state
        if(Time.time - startTime >= stateDuration)
        {
            if(IsGrounded)
                SwitchState(CharacterStateMachine.StateName.WALK);
            else
                SwitchState(CharacterStateMachine.StateName.AIR);
        }
    }

    protected override void OnExitState()
    {
        
    }
}
