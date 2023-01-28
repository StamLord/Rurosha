using UnityEngine;

public class SlideState : PlayerState
{
    [Header("Control Settings")]
    [SerializeField] private AttributeDependant<float> slideDistance;
    [SerializeField] private float slideSpeed = 20f;
    [SerializeField] private AnimationCurve speedOverDistance;
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravity = 20.0f;

    [Space(20f)]

    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private new Rigidbody rigidbody;

    public delegate void SlideStartDelegate();
    public event SlideStartDelegate OnSlideStart;

    public delegate void SlideEndDelegate();
    public event SlideEndDelegate OnSlideEnd;

    private Vector3 slideStart;
    private Vector3 slideDirection;
    private float distance;
    private float startTime;

    void Awake () 
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
	}

    protected override void OnEnterState()
    {
        base.OnEnterState();
        if(debugView) Debug.Log("State: Entered [Slide State]");

        colliderManager.SetBody(ColliderManager.BodyCollider.CROUCH);
        colliderManager.SetLegs(ColliderManager.BodyCollider.CROUCH);
        
        slideStart = rigidbody.position;
        slideDirection = transform.forward;

        rigidbody.velocity = slideDirection * slideSpeed;
        distance = slideDistance.GetValue(characterStats);
        startTime = Time.time;

        if(OnSlideStart != null)
            OnSlideStart();
    }

    public override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if(gravityOn)
            rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
        
        // Fall
        if(IsGrounded == false)
        {
            SwitchState(CharacterStateMachine.StateName.AIR);
            return;
        }

        float distanceTraveled = (rigidbody.position - slideStart).magnitude;

        // Stop slide if traveled enough distance, velocity drops below threshold (For example if hit a wall)
        // OR if enough time has passed (Edge case if we get stuck with velocity not dropping)
        if( distanceTraveled > distance || 
            rigidbody.velocity.magnitude < slideSpeed * .75f ||
            Time.time - startTime >= distance / slideSpeed)
        {
            rigidbody.velocity = Vector3.zero;
            if(IsGrounded)
                SwitchState(CharacterStateMachine.StateName.WALK);
            else
                SwitchState(CharacterStateMachine.StateName.AIR);

        }

        // Update speed
        float t = distanceTraveled / distance;
        rigidbody.velocity = slideDirection * slideSpeed * speedOverDistance.Evaluate(t);
    }

    protected override void OnExitState()
    {
        colliderManager.SetBody(ColliderManager.BodyCollider.STAND);
        colliderManager.SetLegs(ColliderManager.BodyCollider.STAND);

        if(OnSlideEnd != null)
            OnSlideEnd();
    }

}
