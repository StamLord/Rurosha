using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using DitzelGames.FastIK;
using System;

public class SpiderFightState : FightAIState, IHitboxResponder
{
    [Header ("NavMeshAgemnt")]
    [SerializeField] private NavMeshAgent navMeshAgent;

    [Header ("Rigidbody")]
    [SerializeField] private new Rigidbody rigidbody;

    [Header ("Wait Chance")]
    [SerializeField] private float waitChance = .2f;
    [SerializeField] private float waitChanceAfterMovingFor = 2f;
    [SerializeField] private float waitTime = 2f;

    [Header ("Bite Attack")]
    [SerializeField] private float biteAttackCooldown = 2f;
    [SerializeField] private float biteAttackRange = 3f;
    [SerializeField] private AttackInfo biteAttack = new AttackInfo(20, 5);

    [Header ("Jump")]
    [SerializeField] private float jumpMinDistance = 4f;
    [SerializeField] private float jumpMaxDistance = 12f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpChance = 25f;
    [SerializeField] private float jumpTime = .5f;
    [SerializeField] private float groundedOffset = 2f;
    [SerializeField] private float groundedDistance = 1f;
    [SerializeField] private LayerMask groundedMask;
    [SerializeField] private float jumpRecoveryDuration = 1f;
    [SerializeField] private float jumpRecoveryLowestY = .25f;
    private float bodyYPosition = 1f;

    [Header ("Web Shoot")]
    [SerializeField] private Projectile webProjectile;
    [SerializeField] private Transform webOrigin;
    [SerializeField] private float webChance = 25f;
    [SerializeField] private float webMaxDistance = 12f;
    [SerializeField] private float webAimDuration = 2f;
    
    [Header ("Web Spring")]
    [SerializeField] private float webSpring = 100;
    [SerializeField] private Vector3 webAnchor = Vector3.up;
    [SerializeField] private Vector3 webConnectedAnchor = Vector3.up;
    [SerializeField] private float webBreakForce = 1000f;
    [SerializeField] private LineRenderer projectileLineRenderer;
    [SerializeField] private SpringRenderer springRenderer;
    [SerializeField] private ChainManager chainToStatic;
    
    [Header("Web Pull")]
    [SerializeField] private FastIKFabric[] webPullIK;
    [SerializeField] private GameObject webPullVisual;
    [SerializeField] private float webPullRate = 1f;
    [SerializeField] private int maxWebBiteAttacks = 3;
    

    [Header ("Hitbox")]
    [SerializeField] private Hitbox[] hitbox;

    [Header ("Debug")]
    [SerializeField] private float enemyDistance;
    [SerializeField] private FightState currentState;
    [SerializeField] private float stateTime;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isGrounded;

    private bool lastFrameGrounded;
    private bool midFinishJumpCoroutine;

    private bool webHit;
    private Rigidbody webCaughtBody;
    private SpringJoint springJoint;
    private int webBiteAttacks;

    private Transform activeWebProjectile;

    private enum FightState { FOLLOW, WAIT, JUMP_FWD, JUMP_BCK, SHOOT_WEB, WAIT_WEB, PULL_WEB}

    private void Start()
    {
        foreach(Hitbox h in hitbox)
        {
            h.SetResponder(this);
            h.SetIgnoreTransform(transform.root);
        }

        canAttack = true;
        bodyYPosition = rigidbody.position.y;
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();

        if(hurtbox)
            hurtbox.Hit(AIStateMachine.StealthAgent, biteAttack, Vector3.up, Vector3.zero);
    }

    public void GuardedBy(Collider collider, Hitbox hitbox)
    {
        return;
    }

    public void PerfectGuardedBy(Collider collider, Hitbox hitbox)
    {
       return;
    }

    public void UpdateColliderState(bool newState)
    {
        return;
    }

    public override void OnStateUpdate()
    {
        enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
        isGrounded = IsGrounded();

        switch(currentState)
        {
            case FightState.FOLLOW:
                Follow();
                break;
            case FightState.WAIT:
                Wait();
                break;
            case FightState.JUMP_FWD:
                JumpForward();
                break;
            case FightState.JUMP_BCK:
                break;
            case FightState.SHOOT_WEB:
                ShootWeb();
                break;
            case FightState.WAIT_WEB:
                WaitWeb();
                break;
            case FightState.PULL_WEB:
                PullWeb();
                break;
        }
    }

    private void SwitchState(FightState state)
    {
        MoveStop();

        currentState = state;
        stateTime = Time.time;

        bool isWebPull = currentState == FightState.PULL_WEB;
        if(isWebPull)
            Animator.Play("pull_web");
        else
            Animator.Play("idle");
        
        Array.ForEach(webPullIK, x => x.enabled = !isWebPull);
        webPullVisual.SetActive(isWebPull);
    }

    // Follow player and attack when in range
    private void Follow()
    {
        // Rotate to face enemy
        LookTowards(enemy.transform.position);

        Vector3 target = enemy.transform.position; // Default to enemy position
        Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy

        if(enemyDistance < 2f)
            target += dir * 2f;
        else if(enemyDistance > biteAttackRange)
            target += dir * biteAttackRange;
        
        MoveTo(target);

        // Attack if in range
        AttackCheck();

        // If enough time passed, check if we should start waiting
        if(Time.time - stateTime >= waitChanceAfterMovingFor)
        {
            int random = UnityEngine.Random.Range(0, 100);
            if(random <= waitChance)
                SwitchState(FightState.WAIT);
            else
                SwitchState(FightState.FOLLOW); // Resets stateTime
        }
    }
    
    // Don't move, look at player and attack if in range
    private void Wait()
    {
        // Rotate to face enemy
        LookTowards(enemy.transform.position);

        // Attack if in range
        AttackCheck();

        // If enough time passed, check if we should jump or follow
        if(Time.time - stateTime >= waitTime)
        {
            int random = UnityEngine.Random.Range(0, 100);
            if(random <= jumpChance)
            {
                if(enemyDistance >= jumpMinDistance)
                {
                    SwitchState(FightState.JUMP_FWD);
                    return;
                }
            }
            else if(random <= jumpChance + webChance)
            {
                SwitchState(FightState.SHOOT_WEB);
                return;
            }

            SwitchState(FightState.FOLLOW);
        }
    }

    private void JumpForward()
    {
        // Check when jump is finished
        if(isJumping)
        {
            // If this is the first frame we are grounded
            // Or if enough time passed ( Fixes cases where jump was blocked)
            if(isGrounded && lastFrameGrounded == false && midFinishJumpCoroutine == false || midFinishJumpCoroutine == false && Time.time - stateTime >= 5)
            {
                StartCoroutine("FinishJumpCoroutine");
            }
        }
        // Perform Jump
        else
            JumpAtTarget();

        lastFrameGrounded = isGrounded;
    }

    // If close enough, perform bite attack
    private bool AttackCheck()
    {
        if(canAttack == false || enemyDistance > biteAttackRange)
            return false;
        
        BiteAttack();
        return true;
    }

    private void BiteAttack()
    {
        Animator?.Play("bite_attack");
        StartCoroutine(Cooldown(biteAttackCooldown));
    }

    private IEnumerator Cooldown(float cooldown)
    {
        canAttack = false;

        float startTime = Time.time ;

        while(Time.time - startTime <= cooldown)
            yield return null;

        canAttack = true;
    }

    // Apply forces on rigidbody to reach target
    private void JumpAtTarget()
    {
        rigidbody.isKinematic = false;
        navMeshAgent.enabled = false;
        isJumping = true;

        // Calculate target position infront of enemy
        Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy
        Vector3 target = enemy.transform.position + dir * biteAttackRange; // We aim for attack range

        float distance = Vector3.Distance(transform.position, target);
        float horizontalVelocity = distance / jumpTime;
        float verticalVelocity = Mathf.Sqrt(2 * jumpHeight * -Physics.gravity.y);

        // Apply forces to jump at enemy
        Vector3 force = -dir * horizontalVelocity + Vector3.up * verticalVelocity;
        rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    private void FinishJump()
    {
        isJumping = false;
        navMeshAgent.enabled = true;
        rigidbody.isKinematic = true;
    }

    private IEnumerator FinishJumpCoroutine()
    {
        midFinishJumpCoroutine = true;
        rigidbody.isKinematic = true;

        float timeStart = Time.time;

        while(Time.time - timeStart < jumpRecoveryDuration)
        {
            // Update body position
            float p = (Time.time - timeStart) / jumpRecoveryDuration;
            float y = (p <= .5f) ? Mathf.Lerp(bodyYPosition, jumpRecoveryLowestY, p) : Mathf.Lerp(jumpRecoveryLowestY, bodyYPosition, p);
            rigidbody.position = new Vector3(rigidbody.position.x, y, rigidbody.position.z);

            yield return null;
        }

        // Reset body position
        rigidbody.position = new Vector3(rigidbody.position.x, bodyYPosition, rigidbody.position.z);
        rigidbody.isKinematic = false;

        FinishJump();
        SwitchState(FightState.WAIT);

        midFinishJumpCoroutine = false;
    }
    
    private void ShootWeb()
    {
        // Face away from enemy
        Vector3 dir = (transform.position - enemy.transform.position).normalized;
        LookTowards(transform.position + dir);

        // Wait before shooting web
        if(Time.time - stateTime < webAimDuration)
            return;

        // Create Projectile
        if(webProjectile)
        {   
            Vector3 origin = webOrigin != null? webOrigin.position : transform.position;
            Projectile proj = Instantiate(webProjectile, origin, Quaternion.identity);
            proj.OnProjecitleStop += WebProjectileHit;
            proj.transform.forward = enemy.transform.position + Vector3.up - origin;

            activeWebProjectile = proj.transform;

            // Start lineRenderer for projectile
            projectileLineRenderer.enabled = true;
            projectileLineRenderer.positionCount = 2;
            projectileLineRenderer.SetPosition(0, webOrigin.position);
            projectileLineRenderer.SetPosition(1, activeWebProjectile.position);
        }

        SwitchState(FightState.WAIT_WEB);
    }

    private void WaitWeb()
    {
        // Use lineRenderer to draw web from us to projectile
        projectileLineRenderer.SetPosition(0, webOrigin.position);
        projectileLineRenderer.SetPosition(1, activeWebProjectile.position);

        // Safeguard against edge cases
        if(Time.time - stateTime >= 10f)
            SwitchState(FightState.WAIT);
    }

    private void WebProjectileHit(RaycastHit hit)
    {   
        // Turn off lineRender for projetile
        projectileLineRenderer.enabled = false;

        Hurtbox hurtbox = hit.collider.transform.GetComponent<Hurtbox>();
        Rigidbody rb = hit.collider.transform.GetComponent<Rigidbody>();
        //Debug.Log(String.Format("Hit: {0} Hurtbox: {0} Rigidbody: {1}", hit.collider.name, (hurtbox != null)? hurtbox.gameObject.name : "Null", (rb != null)? rb.gameObject.name : "Null"));

        // Scenerio 1: Hit an enemy target
        if(hurtbox)
        {
            // Get rigidbody of enemy
            //
            // Enemy Parent
            // ↳ Alive (Rigidbody is here)
            //   ↳ Hurtboxes
            //     ↳ Hurtbox (We get this above)

            rb = hit.collider.transform.parent.parent.GetComponent<Rigidbody>();
            if(rb == null) return;

            CreateWebSpring(rb);
        }
        // Scenerio 2: Hit a wall or object
        else if(rb == null)
        {
            CreateWebSpring(hit.point);
        }

        SwitchState(FightState.PULL_WEB);
    }

    /// <summary>
    /// Creates a spring joint and sets a spring renderer between a rigidbody and ourselves
    /// </summary>
    private void CreateWebSpring(Rigidbody rigidbody)
    {
        // Create spring joint
        springJoint = gameObject.AddComponent<SpringJoint>();

        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.anchor = webAnchor;
        springJoint.connectedBody = rigidbody;
        springJoint.connectedAnchor = webConnectedAnchor;
        springJoint.spring = webSpring;
        springJoint.breakForce = webBreakForce;
        springJoint.maxDistance = Vector3.Distance(transform.position, rigidbody.position); // Spring length is current distance

        springRenderer.SetSpring(springJoint);
    }
    /// <summary>
    /// Creates a prefab of a web, and sets the last child at a worldSpace position.
    /// </summary>
    private void CreateWebSpring(Vector3 position)
    {
        if(chainToStatic == null) return;

        GameObject web = Instantiate(chainToStatic.gameObject, position, Quaternion.identity);
        
        // Initialize chain
        ChainManager chainManager = web.GetComponent<ChainManager>();
        chainManager.InitializeChain(webOrigin.position, position);

        // Set last link to be kinematic
        chainManager[chainManager.Count-1].Rigidbody.isKinematic = true;
    }

    private void PullWeb()
    {
        if(springJoint == null)
        {
            SwitchState(FightState.WAIT);
            return;
        }
        
        // Rotate to face enemy
        LookTowards(springJoint.connectedBody.position);

        // Activate visual after enough time to turn around
        if(Time.time - stateTime >= 1f)
            webPullVisual.SetActive(true);
        
        // Shorten the web to pull target
        springJoint.maxDistance -= webPullRate * Time.deltaTime;

        // If close enough, attack
        if(Vector3.Distance(transform.position, springJoint.connectedBody.position) < biteAttackRange)
        {
            if(AttackCheck())
                webBiteAttacks++;
        }

        if(webBiteAttacks >= maxWebBiteAttacks)
        {   
            webBiteAttacks = 0;
            springRenderer.DestroySpring();
            SwitchState(FightState.WAIT);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * groundedOffset, Vector3.down, groundedDistance, groundedMask);
    }

    protected override void LoseAgent(StealthAgent agent)
    {
        // Ignore any agent except our enemy
        if(agent != enemy) return;

        if(currentState == FightState.PULL_WEB || currentState == FightState.SHOOT_WEB || currentState == FightState.JUMP_FWD) return; // Can't lose sight when pulling the enemy

        // Start searching
        AIStateMachine.enemyLastSeen = agent.transform.position; // Last position
        Rigidbody rb = agent.GetComponent<Rigidbody>();
        AIStateMachine.enemyLastDir = (rb)? rb.velocity : Vector3.zero; // Last direction
        AIStateMachine.enemyLastDir.y = 0; // Flatten direction to discard jumping / falling vectors

        // Switch to SearchAIState
        SwitchState(AIStateMachine.StateName.SEARCH);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position + Vector3.up * groundedOffset, transform.position + Vector3.up * groundedOffset + Vector3.down * groundedDistance);
    }

}
