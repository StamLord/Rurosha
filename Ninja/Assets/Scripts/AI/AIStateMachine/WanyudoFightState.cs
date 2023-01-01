using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanyudoFightState : FightAIState, IHitboxResponder
{
    [SerializeField] private Animator animator;
    
    [SerializeField] private float attackCooldown = 2f;

    [Header ("Spin attack")]
    [SerializeField] private float spinRange = 2f;
    [SerializeField] private float spinDuration = 2f;
    [SerializeField] private float spinCooldown = 1f;

    [Header ("Charge attack")]
    [SerializeField] private float preChargeTime = 1f;
    [SerializeField] private float chargeDistance = 3f;
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float chargeCooldown = 1f;
    [SerializeField] private ParticleSystem fireTrail;

    [Header ("Hitbox")]
    [SerializeField] private Hitbox[] hitbox;
    [SerializeField] private int softDamage;
    [SerializeField] private int hardDamage;
    
    [Header ("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Vector2 pitchVariation = new Vector2(.5f, 1f);

    private bool midAttack;
    private bool canMove = true;
    private bool canRotate = true;
    private bool canLoseEnemy = true;

    private float lastAttack;
    private bool delayedLoseAgent;
    private int dashCounter;
    
    private Dictionary<Hurtbox, int> hitsRegistered;

    private void Start() 
    {
        foreach(Hitbox h in hitbox)
        {
            h.SetResponder(this);
            h.SetIgnoreTransform(transform.root);
        }
    }

    protected override void DrawWeapon()
    {
        // No weapons
    }

    public override void OnStateUpdate()
    {
        // If we lost enemy during an attack and had to delay the event
        if(canLoseEnemy && delayedLoseAgent)
        {
            delayedLoseAgent = false;
            LoseEnemy();
        }

        // If close enough, AI faces player
        if(canRotate)
        {
            if(TooFar(enemy.transform.position, faceEnemyDistance))
                LookTowards(AIStateMachine.GetNextPosition());
            else
                LookTowards(enemy.transform.position);
        }

        if(canMove)
        {
            Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy
            
            // Get point at needed distance from enemy

            Vector3 target = transform.position;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            
            if(distance < 2f)
                target = enemy.transform.position + dir * 2f;
            else if(distance > attackRange)
                target = enemy.transform.position + dir * attackRange;
            

            // Recalculate if target position changed enough
            if(Vector3.Distance(target, lastPathTarget) > pathRecalculateDistance ||
                Vector3.Distance(transform.position, target) >= .3f)
            {
                lastPathTarget = target;
                MoveTo(target);
            }
        }

        // Can attack (based on TargetManager) and not already attacking
        if (canAttack && midAttack == false && Time.time - lastAttack > attackCooldown)
            Attack();
    }

    protected override void Attack()
    {
        // Spin if in close range with enemy
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        
        // Spin attack range
        if(distance <= spinRange)
        {
            // Start if enough time passed since last attack
            if(Time.time - lastAttack > spinCooldown)
                StartCoroutine("Spin");
        }
        // Choose between dash and flamethrower
        else
        {
            // Roll random number to select attack
            float rand = Random.Range(0f,1f);
            
            if(rand <= 80 && Time.time - lastAttack > chargeCooldown)
                StartCoroutine("Dash");
        }
    }

    private IEnumerator Spin()
    {
        midAttack = true;
        canMove = false;

        // Select random spin animation
        float rand = Random.Range(0f, 1f);
        
        if(rand <= .5f)
            animator.Play("spin");
        else
            animator.Play("spin_2");

        // Wait for duration of spin
        float startTime = Time.time;
        while( Time.time - startTime < spinDuration)
            yield return null;

        animator.CrossFade("idle", .1f);
        
        midAttack = false;
        canMove = true;
        lastAttack = Time.time;
    }

    private IEnumerator Dash()
    {
        midAttack = true;
        canMove = false;

        // Check which type of dash this is
        bool dash2 = (dashCounter == 2);

        // Start dash prepare
        float startTime = Time.time;
        if(dash2)
            animator.Play("pre_dash_2");
        else
            animator.Play("pre_dash");

        // Sound
        if(audioSource)
        {
            if(audioSource.isPlaying == false)
            {   
                audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
                audioSource.Play();
            }
        }

        // Continue aiming towards player while preparing
        while( Time.time - startTime < preChargeTime)
        {
            LookTowards(enemy.transform.position);
            yield return null;
        }

        // Perform dash
        if(dash2)
            animator.Play("dash_2");
        else
            animator.Play("dash");

        // Start fire trail vfx
        fireTrail?.Play();
        
        // Lock rotation so we dash in straight line
        canRotate = false;

        // Can't lose player once we start dash
        canLoseEnemy = false;

        // Get direction to enemy and set it in inputState
        Vector3 direction = enemy.transform.position - transform.position;
        direction = transform.InverseTransformDirection(direction);
        StartOverrideMovement(direction);

        // We simulate run button to move faster
        HoldButton("run");
    
        float distanceTraveled = 0;
        float dashStartTime = Time.time;
        Vector3 lastPosition = transform.position;

        // We dash until either dashDistance is covered or maxDashTime passes
        while (distanceTraveled < chargeDistance && Time.time - dashStartTime < maxChargeTime)
        {
            distanceTraveled += Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;
            yield return null;
        }

        // Stop dash
        StopHoldButton("run");

        if(dash2)
            animator.Play("end_dash_2");
        else
            animator.Play("end_dash");

        StopOverrideMovement();

        // Stop fire trail vfx
        fireTrail?.Stop();

        midAttack = false;
        canMove = canRotate = canLoseEnemy = true;
        lastAttack = Time.time;

        if(dash2)
            dashCounter = 0;
        else
            dashCounter++;
    }

    protected override void LoseAgent(StealthAgent agent)
    {
        // Ignore any agent except our enemy
        if(agent != enemy) return;

        // We can't lose player (Like mid dash)
        if(canLoseEnemy == false)
        {
            delayedLoseAgent = true;
            return;
        }

        LoseEnemy();
    }

    private void LoseEnemy()
    {
        // Start searching
        AIStateMachine.enemyLastSeen = enemy.transform.position; // Last position
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        AIStateMachine.enemyLastDir = (rb)? rb.velocity : Vector3.zero; // Last direction
        AIStateMachine.enemyLastDir.y = 0; // Flatten direction to discard jumping / falling vectors

        // Switch to SearchAIState
        AIStateMachine.SwitchState(2);
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        //Hurtbox
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if(hurtbox)
        {
            hurtbox.Hit(AIStateMachine.StealthAgent, softDamage, hardDamage, DamageType.Blunt);
            if(hitsRegistered.ContainsKey(hurtbox))
            {
                if(hitsRegistered[hurtbox] == 3)
                {
                    Rigidbody rigid = collider.GetComponent<Rigidbody>();
                    if(rigid) 
                    {
                        Vector3 dir = collider.transform.position - transform.position;
                        dir.Normalize();
                        rigid.AddForce(dir * 10f, ForceMode.VelocityChange);
                    }
                    hitsRegistered[hurtbox] = 0;
                }
                else
                    hitsRegistered[hurtbox]++;
            }
            else 
                hitsRegistered[hurtbox] = 1;

        }

    }
     
    public void GuardedBy(Collider collider, Hitbox hitbox)
    {

    }

    public void UpdateColliderState(bool newState)
    {

    }
}
