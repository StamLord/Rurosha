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
    [SerializeField] private float waitTime = 2f;

    [Header ("Bite Attack")]
    [SerializeField] private float biteAttackCooldown = 2f;
    [SerializeField] private float biteAttackRange = 2f;

    [Header ("Bite Damage")]
    [SerializeField] private int biteSoftDamage = 20;
    [SerializeField] private int biteHardDamage = 5;

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

    [SerializeField] private bool isWaiting;
    [SerializeField] private bool canAttack;
    [SerializeField] private bool isJumping;
    private bool lastFrameGrounded;

    private void Start()
    {
        foreach(Hitbox h in hitbox)
        {
            h.SetResponder(this);
            h.SetIgnoreTransform(transform.root);
        }

        isWaiting = false;
        canAttack = true;
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();

        if(hurtbox)
            hurtbox.Hit(AIStateMachine.StealthAgent, biteSoftDamage, biteHardDamage, Vector3.up, DamageType.Blunt);
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
        bool grounded = IsGrounded();

        // If mid jump or fall, do nothing
        
        // Activate navMeshWhen when we land ( We deactivate it in JumpAtTarget)
        if(grounded == true && lastFrameGrounded == false)
        {
            navMeshAgent.enabled = true;
            isJumping = false;
        }

        lastFrameGrounded = grounded;

        if(grounded == false)
            return;
        
        // Distance between us and enemy
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        
        // If close enough, perform bite attack
        if(canAttack && distance <= biteAttackRange)
        {
            BiteAttack();
            return;
        }
        
        // If waiting, only rotate towards enemy
        if(isWaiting)
        {
            LookTowards(enemy.transform.position);
            return;
        }

        // Roll for a chance to wait only when not moving
        int random = Random.Range(0, 100);
        if(random <= waitChance)
        {
            Wait();
            return;
        }

        // Roll for a chance to jump at target
        if(isJumping == false)            
        {
            // Within jumping distance
            if( distance >= jumpMinDistance
                && distance <= jumpMaxDistance)
            {
                random = Random.Range(0, 100);
                if(random <= jumpChance)
                {
                    JumpAtTarget();
                    return;
                }
            }
        }

        // Follow player normally
        Vector3 target = enemy.transform.position; // Default to enemy position
        Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy

        if(distance < 2f)
            target += dir * 2f;
        else if(distance > attackRange)
            target += dir * attackRange;
        
        MoveTo(target);
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

    private void JumpAtTarget()
    {
        isJumping = true;
        navMeshAgent.enabled = false;

        // Calculate target position infront of enemy
        Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy
        Vector3 target = enemy.transform.position + dir * attackRange; // We aim for attack range

        float distance = Vector3.Distance(transform.position, target);
        float horizontalVelocity = distance / jumpTime;
        float verticalVelocity = Mathf.Sqrt(2 * jumpHeight * -Physics.gravity.y);

        // Apply forces to jump at enemy
        Vector3 force = -dir * horizontalVelocity + Vector3.up * verticalVelocity;
        Debug.Log(force);
        rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    private void Wait()
    {
        StartCoroutine(Waiting());
    }

    private IEnumerator Waiting()
    {
        isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        isWaiting = false;
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
