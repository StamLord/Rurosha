using System.Collections;
using UnityEngine;

public class KappaFightState : FightAIState, IHitboxResponder
{
    [Header ("Animator")]
    [SerializeField] private Animator animator;

    [Header ("Hitbox")]
    [SerializeField] private Hitbox[] hitbox;

    [Header ("Melee")]
    [SerializeField] private int meleeSoftDamage = 7;
    [SerializeField] private int meleeHardDamage = 5;
    [SerializeField] private Status[] meleeStatuses;
    [SerializeField] private float meleeAttackRange = 2;
    [SerializeField] private float meleeAttackCooldown = 1;
    [SerializeField] private float minimumTimeInMelee = 3;

    [Header ("Ranged")]
    [SerializeField] private int safeDistance = 10;
    [SerializeField] private float maximumKeepDistanceTime = 3;
    [SerializeField] private float projecitleCost = 10;
    [SerializeField] private float castDuration = 3f;
    [SerializeField] private float castCancleDistance = 3f;
    [SerializeField] private ChakraType projectileChakra = ChakraType.WATER;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileOrigin;
    [SerializeField] private GameObject visualCastObject;

    [Header ("Debug")]
    [SerializeField] private float enemyDistance;
    [SerializeField] private FightState currentState;
    [SerializeField] private float stateTime;

    private enum FightState {KEEP_DISTANCE, CAST, MELEE}
    private Coroutine castingCoroutine;
    private bool isCasting;

    private void Start()
    {
        foreach(Hitbox h in hitbox)
        {
            h.SetResponder(this);
            h.SetIgnoreTransform(transform.root);
        }

        canAttack = true;
    }

    protected override void OnEnterState()
    {
        enemy = (AIStateMachine.enemy)? AIStateMachine.enemy : null;
        AIStateMachine.AwarenessAgent.OnLoseAgent += LoseAgent;

        SwitchState(FightState.KEEP_DISTANCE);
    }

    public override void OnStateUpdate()
    {
        enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);

        switch(currentState)
        {
            case FightState.KEEP_DISTANCE:
                KeepDistance();
                break;
            case FightState.CAST:
                Cast();
                break;
            case FightState.MELEE:
                Melee();
                break;
        }
    }

    private void KeepDistance()
    {
        // If not enough mana, or spent too long in this state, switch to melee attacks
        if( Stats.GetChakraAmount(projectileChakra) < projecitleCost ||
            Time.time - stateTime > maximumKeepDistanceTime)
        {
            SwitchState(FightState.MELEE);
            return;
        }

        // If in safe distance we start casting
        if(enemyDistance >= safeDistance)
        {
            SwitchState(FightState.CAST);
            return;
        }

        // Move to safe position
        Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy
        Vector3 target = transform.position + dir; // Position away from enemy

        // Rotate towards the target
        LookTowards(target);
        
        // Move to target
        MoveTo(target);
    }

    private void Cast()
    {
        // Rotate to face enemy
        LookTowards(enemy.transform.position);

        if(isCasting)
        {
            // If enemy too close, cancel cast and gain distance
            if(enemyDistance <= castCancleDistance)
                SwitchState(FightState.KEEP_DISTANCE);

            return;
        }

        // If not enough mana, switch to melee attacks
        if(Stats.GetChakraAmount(projectileChakra) < projecitleCost)
            SwitchState(FightState.MELEE);

        // If enough mana, start casting
        else
        {
            // Reduce chakra
            Stats.DepleteChakra(projectileChakra, projecitleCost);
            
            // Start casting
            castingCoroutine = StartCoroutine("CastingCoroutine");
        }
    }

    private void Melee()
    {
        if( Time.time - stateTime >= minimumTimeInMelee && // Stay atleast x time in melee state
            Stats.GetChakraAmount(projectileChakra) >= projecitleCost) // If enough mana to cast, keep distance
        {
            SwitchState(FightState.KEEP_DISTANCE);
            return;
        }

        // Rotate to face enemy
        LookTowards(enemy.transform.position);

        Vector3 target = enemy.transform.position; // Default to enemy position
        Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy

        target += dir * meleeAttackRange;
        
        MoveTo(target);

        // Attack if in range
        AttackCheck();
    }

    private void SwitchState(FightState state)
    {
        currentState = state;
        stateTime = Time.time;

        if(isCasting)
            CancelCasting();
    }

    // If close enough, perform bite attack
    private void AttackCheck()
    {
        if( canAttack == false || 
            enemyDistance > meleeAttackRange + .2f) // Allow for .2 unit deviation in case we stopped short of melee range
            return;
        
        MeleeAttack();
    }

    private void MeleeAttack()
    {
        animator.Play("attack");
        StartCoroutine(Cooldown(meleeAttackCooldown));
    }

    private IEnumerator Cooldown(float cooldown)
    {
        canAttack = false;

        float startTime = Time.time ;

        while(Time.time - startTime <= cooldown)
            yield return null;

        canAttack = true;
    }

    private void CastProjectile()
    {
        // Origin of proejctile, either a reference or 1 unit from our origin (floor)
        Vector3 origin = (projectileOrigin)? projectileOrigin.position : transform.position + Vector3.up;

        // Instantiate and orient projectile object
        Vector3 target = enemy.transform.position;
        // Direction to target with offset of 1.5 unit up
        Vector3 dir = (target + Vector3.up * 1.5f - origin).normalized;

        GameObject go = Instantiate(projectile, origin, Quaternion.LookRotation(dir, Vector3.up));
        Projectile proj = go.GetComponent<Projectile>();

        // Setup projectile
        if(proj)
        {
            proj.SetOwner(StealthAgent);
            proj.SetIgnoreTransform(transform.root);
        }
    }

    private IEnumerator CastingCoroutine()
    {
        isCasting = true;

        // Stop moving
        MoveStop();

        animator?.Play("cast_start");
        visualCastObject?.SetActive(true);

        // Casting time
        float startTime = Time.time;
        while(Time.time - startTime <= castDuration)
        {
            visualCastObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (Time.time - startTime) / castDuration);
            yield return null;
        }

        animator?.Play("cast_end");
        visualCastObject?.SetActive(false);

        // Creates projectile
        CastProjectile();

        // Give casting animation time to finish
        yield return new WaitForSeconds(1f);

        isCasting = false;
    }

    private void CancelCasting()
    {
        // Regain chakra
        Stats.ChargeChakra(projectileChakra, projecitleCost);

        if(castingCoroutine != null)
            StopCoroutine(castingCoroutine);
        
        isCasting = false;
        visualCastObject?.SetActive(false);
        
        // Cancel animation
        animator?.Play("grounded");
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();

        if(hurtbox)
            hurtbox.Hit(AIStateMachine.StealthAgent, meleeSoftDamage, meleeHardDamage, Vector3.up, DamageType.Blunt, meleeStatuses);
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

    protected override void LoseAgent(StealthAgent agent)
    {
        // Ignore any agent except our enemy
        if(agent != enemy) 
            return;

        // Allow a cheat range where we can't lose enemy
        if(enemyDistance <= awarenessCheatRange)
            return;

        // Don't care about losing agent if we are running away to get to a safe palce
        if(currentState == FightState.KEEP_DISTANCE)
            return;

        // Start searching
        AIStateMachine.enemyLastSeen = agent.transform.position; // Last position
        Rigidbody rb = agent.GetComponent<Rigidbody>();
        AIStateMachine.enemyLastDir = (rb)? rb.velocity : Vector3.zero; // Last direction
        AIStateMachine.enemyLastDir.y = 0; // Flatten direction to discard jumping / falling vectors

        // Switch to SearchAIState
        SwitchState(AIStateMachine.StateName.SEARCH);
    }

    protected override void OnExitState()
    {
        // Cancel casting before switching states
        if(isCasting)
            CancelCasting();
    }
}
