using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderFightState : FightAIState, IHitboxResponder
{
    [Header ("NavMeshAgemnt")]
    [SerializeField] private NavMeshAgent navMeshAgent;

    [Header ("Rigidbody")]
    [SerializeField] private Rigidbody rigidbody;

    [Header ("Animator")]
    [SerializeField] private Animator animator;

    [Header ("Wait Chance")]
    [SerializeField] private float waitChance = .2f;
    [SerializeField] private float waitChanceAfterMovingFor = 2f;
    [SerializeField] private float waitTime = 2f;

    [Header ("Bite Attack")]
    [SerializeField] private float biteAttackCooldown = 2f;
    [SerializeField] private float biteAttackRange = 3f;

    [Header ("Bite Damage")]
    [SerializeField] private int biteSoftDamage = 20;
    [SerializeField] private int biteHardDamage = 5;

    [Header ("Bite Status Afflictions")]
    [SerializeField] private Status[] biteStatuses;

    [Header ("Jump")]
    [SerializeField] private float jumpMinDistance = 4f;
    [SerializeField] private float jumpMaxDistance = 12f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpChance = .5f;
    [SerializeField] private float jumpTime = .5f;
    [SerializeField] private float groundedOffset = 2f;
    [SerializeField] private float groundedDistance = 1f;
    [SerializeField] private LayerMask groundedMask;

    [Header ("Hitbox")]
    [SerializeField] private Hitbox[] hitbox;

    [Header ("Debug")]
    [SerializeField] private float enemyDistance;
    [SerializeField] private FightState currentState;
    [SerializeField] private float stateTime;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isGrounded;

    private bool lastFrameGrounded;
    
    private enum FightState { FOLLOW, WAIT, JUMP_FWD, JUMP_BCK}

    private void Start()
    {
        foreach(Hitbox h in hitbox)
        {
            h.SetResponder(this);
            h.SetIgnoreTransform(transform.root);
        }

        canAttack = true;
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();

        if(hurtbox)
            hurtbox.Hit(AIStateMachine.StealthAgent, biteSoftDamage, biteHardDamage, Vector3.up, DamageType.Blunt, biteStatuses);
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
        }
        
        
    }

    private void SwitchState(FightState state)
    {
        currentState = state;
        stateTime = Time.time;
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
        else if(enemyDistance > attackRange)
            target += dir * attackRange;
        
        MoveTo(target);

        // Attack if in range
        AttackCheck();

        // If enough time passed, check if we should start waiting
        if(Time.time - stateTime >= waitChanceAfterMovingFor)
        {
            int random = Random.Range(0, 100);
            if(random <= waitChance)
                SwitchState(FightState.WAIT);
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
            if(enemyDistance >= jumpMinDistance)
            {
                int random = Random.Range(0, 100);
                if(random <= jumpChance)
                {
                    SwitchState(FightState.JUMP_FWD);
                    return;
                }
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
            if(isGrounded && lastFrameGrounded == false || Time.time - stateTime >= 5)
            {
                FinishJump();
                SwitchState(FightState.WAIT);
            }
        }
        // Perform Jump
        else
            JumpAtTarget();

        lastFrameGrounded = isGrounded;
    }

    // If close enough, perform bite attack
    private void AttackCheck()
    {
        if(canAttack == false || enemyDistance > biteAttackRange)
            return;
        
        BiteAttack();
    }

    private void BiteAttack()
    {
        animator.Play("bite_attack");
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

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * groundedOffset, Vector3.down, groundedDistance, groundedMask);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position + Vector3.up * groundedOffset, transform.position + Vector3.up * groundedOffset + Vector3.down * groundedDistance);
    }

}
