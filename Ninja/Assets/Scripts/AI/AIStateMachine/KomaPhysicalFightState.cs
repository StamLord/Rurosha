using System.Collections;
using UnityEngine;

public class KomaPhysicalFightState : FightAIState, IHitboxResponder
{
    private enum SubState { CIRCLE, 
                            MELEE,
                            CHARGED_ATTACK,
                            CAST_BALL_WAIT,
                            CAST_BALL_READY 
                            }

    public enum MessageType { LOW_HEALTH, ALLY_DEATH }

    [Header ("Ally")]
    [SerializeField] private KomaMagicFightState ally;

    [Header ("Melee")]
    [SerializeField] private float meleeDuration = 10;

    [Header ("Claw Attack")]
    [SerializeField] private AttackInfo clawAttackInfo = new AttackInfo(7, 4, DamageType.Slash);
    [SerializeField] private float clawAttackRange = 2;
    [SerializeField] private float clawAttackCooldown = 1;
    [SerializeField] private Hitbox clawHitbox;

    [Header ("Tail Attack")]
    [SerializeField] private AttackInfo tailAttackInfo = new AttackInfo(7, 4, DamageType.Blunt);
    [SerializeField] private float tailAttackRange = 2;
    [SerializeField] private float tailAttackCooldown = 1;
    [SerializeField] private Hitbox tailHitbox;
    [SerializeField] private float tailForce = 10;

    [Header ("Smash Attack")]
    [SerializeField] private AttackInfo smashAttackInfo = new AttackInfo(7, 4, DamageType.Blunt);
    [SerializeField] private float smashAttackRange = 2;
    [SerializeField] private float smashAttackCooldown = 1;
    [SerializeField] private Hitbox smashHitbox;
    [SerializeField] private float smashForce = 10;

    [Header ("Charged Attack")]
    [SerializeField] private float chargeMinAttackDistance = 7;
    [SerializeField] private float chargeMaxAttackDistance = 12;
    [SerializeField] private float chargeAnimationLength = .91f;
    [SerializeField] private float chargeSpeed = 14;
    [SerializeField] private float chargeAttackCooldown = 4f;

    [Header ("Circle")]
    [SerializeField] private float circleDuration = 5;
    [SerializeField] private float circleRadius = 5;
    [SerializeField] private float circleSpeed = 5;
    [SerializeField] private float currentCircleAngle = 0;

    [Header ("SubState")]
    [SerializeField] private SubState subState;
    [SerializeField] private float subStateTime;
    
    private float enemyDistance;
    private bool midChargeAttack;
    private float lastChargeAttack;
    private int meleeAttackChain;

    private void Start()
    {
        clawHitbox.SetResponder(this, transform.root);
        tailHitbox.SetResponder(this, transform.root);
        smashHitbox.SetResponder(this, transform.root);
    }

    protected override void OnEnterState()
    {
        canAttack = true;
        
        // Sync enemy with ally
        if(ally)
            enemy = ally.GetEnemy() ?? AIStateMachine.enemy ;
        else
            enemy = AIStateMachine.enemy;
        
        Stats.OnHit += Hit;
        Stats.OnDeath += Death;

        if(SquadAgent != null) SquadAgent.OnGetMessage += GetMessage;

        SwitchState(SubState.CIRCLE);
    }

    public override void OnStateUpdate()
    {
        enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);

        switch(subState)
        {
            case SubState.CIRCLE:
                Circle();
                break;
            case SubState.MELEE:
                Melee();
                break;
            case SubState.CHARGED_ATTACK:
                ChargedAttack();
                break;
            case SubState.CAST_BALL_WAIT:
                BallWait();
                break;
            case SubState.CAST_BALL_READY:
                BallReady();
                break;
        }
    }

    protected override void OnExitState()
    {
        if(SquadAgent != null) SquadAgent.OnGetMessage -= GetMessage;
        Stats.OnHit -= Hit;
        Stats.OnDeath -= Death;
    }

    private void SwitchState(SubState state)
    {
        Debug.Log("Switching from " + subState + " to: " + state);
        subState = state;
        subStateTime = Time.time;
    }

    public void GetMessage(string message, SquadAgent sender)
    {

    }

    private void Hit(int softDamage, int hardDamage)
    {
        // Bellow 50%
        if(Stats.Health / Stats.MaxHealth < .5f)
            SquadAgent.SendMessage(MessageType.LOW_HEALTH.ToString());
    }

    private void Death()
    {
        SquadAgent.SendMessage(MessageType.ALLY_DEATH.ToString());
    }

    private void Circle()
    {
        Vector3 target = enemy.transform.position; // Default to enemy position

        //currentCircleAngle += Time.deltaTime * circleSpeed; // Increment angle around enemy
        //currentCircleAngle %= 360f; // Reset at 360

        // Target moves around the player on the y axis
        
        target += Quaternion.Euler(0, currentCircleAngle, 0) * Vector3.left * circleRadius;
        Debug.DrawLine(enemy.transform.position, target, Color.red);
        
        // Move to target
        MoveTo(target);

        if(Time.time - subStateTime > circleDuration)
            SwitchState(SubState.MELEE);
    }

    private bool CanAttack()
    {
        if(canAttack == false) return false;
        
        float range = meleeAttackChain;

        switch(meleeAttackChain)
        {
            case 0:
                range = clawAttackRange;
                break;
            case 1:
                range = tailAttackRange;
                break;
            case 2:
                range = smashAttackRange;
                break;
        }

        // Allow for .2 unit deviation in case we stopped short of melee range
        return enemyDistance < range + .2f;
    }

    private void AttemptMeleeAttack()
    {
        if(CanAttack() == false) return;
        
        switch(meleeAttackChain)
        {
            case 0:
                Animator?.Play("claw_attack");
                StartCoroutine(Cooldown(clawAttackCooldown));
                break;
            case 1:
                Animator?.Play("tail_attack");
                StartCoroutine(Cooldown(tailAttackCooldown));
                break;
            case 2:
                Animator?.Play("smash_attack");
                StartCoroutine(Cooldown(smashAttackCooldown));
                break;
        }

        meleeAttackChain = (meleeAttackChain + 1) % 3;
    }

    private void AttemptChargeAttack()
    {
        if(canAttack == false) return;
        if(Time.time - lastChargeAttack < chargeAttackCooldown) return;

        StartCoroutine("ChargeAttackCoroutine");
    }

    private IEnumerator Cooldown(float cooldown)
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }

    public void StartMelee()
    {
        SwitchState(SubState.MELEE);
    }

    private void Melee()
    {
        // Rotate to face enemy
        LookTowards(enemy.transform.position);

        Vector3 target = enemy.transform.position; // Default to enemy position
        Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy

        target += dir * clawAttackRange;
        
        // Move to enemy
        MoveTo(target);

        // Attack if in range and not attacking
        AttemptMeleeAttack();

        // If far enough, switch to charged attack
        if(enemyDistance >= chargeMinAttackDistance)
        {
            SwitchState(SubState.CHARGED_ATTACK);
            return;
        }
        
        if(Time.time - subStateTime > meleeDuration)
            SwitchState(SubState.CIRCLE);
    }

    private void ChargedAttack()
    {
        if(midChargeAttack)
            return;
        
        // Rotate to face enemy
        LookTowards(enemy.transform.position);

        // If too far, approach
        if(enemyDistance > chargeMaxAttackDistance)
        {
            Vector3 target = enemy.transform.position; // Default to enemy position
            Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy

            target += dir * chargeMinAttackDistance;

            // Move to enemy
            MoveTo(target);
            return;
        }
        // If to close, switch to melee
        else if (enemyDistance < chargeMinAttackDistance)
        {
            SwitchState(SubState.MELEE);
            return;
        }

        // At right distance, perform charge attack
        AttemptChargeAttack();
    }

    private IEnumerator ChargeAttackCoroutine()
    {
        canAttack = false;
        midChargeAttack = true;
        lastChargeAttack = Time.time;

        Animator?.Play("charge_attack");
        
        Vector3 startPos = transform.position;
        Vector3 endPos = enemy.transform.position;

        // Fix end pos to navMesh
        Vector3 navMeshEndPos;
        bool onNavMesh = SamplePosition(endPos, out navMeshEndPos);

        if(onNavMesh)
            endPos = navMeshEndPos;

        Vector3 direction = (endPos - startPos);
        float distance = direction.magnitude;
        direction.Normalize();

        float duration = distance / chargeSpeed;
        duration = Mathf.Min(duration, chargeAnimationLength);

        float startTime = Time.time;

        MoveStop();
        
        while(Time.time - startTime <= duration)
        {
            if(Input.GetKey(KeyCode.F))
                Debug.Break();

            // Check path
            RaycastHit hit;
            if(Physics.Raycast(transform.position, direction, out hit, 2f))
                break;

            // Rotate to face enemy
            LookTowards(enemy.transform.position, 20f);

            // Move
            transform.position = Vector3.Lerp(startPos, endPos, (Time.time - startTime) / duration);

            yield return null;
        }
        
        canAttack = true;
        midChargeAttack = false;
        Animator?.CrossFade("idle_awake", .1f);
        
        MoveTo(enemy.transform.position);
    }

    public void StartBallWait()
    {
        SwitchState(SubState.CAST_BALL_WAIT);
    }

    /// <summary>
    /// Waiting for ally to cast a ball projectile
    /// </summary>
    private void BallWait()
    {
        // Rotate to face ally
        if(ally)
            LookTowards(ally.transform.position);
        
        // We  stand behind enemy to throw back the ball
        Vector3 target = enemy.transform.position; // Default to enemy position
        Vector3 dir = enemy.transform.position - ally.transform.position; // Vector from ally to enemy
        target += dir;

        // Move to target
        MoveTo(target);
    }

    public void StartBallReady()
    {
        MoveStop();

        Animator.Play("ball_wait");

        SwitchState(SubState.CAST_BALL_READY);
    }

    /// <summary>
    /// Ready to bounce back a ball projectile towards enemy
    /// </summary>
    private void BallReady()
    {
        if(enemy)
            LookTowards(enemy.transform.position);
    }

    public void BallBounceAnimation()
    {
        Animator.Play("ball_bounce");
    }
    
    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if(hurtbox == null) return;

        if(hitbox == clawHitbox)
            hurtbox.Hit(AIStateMachine.StealthAgent, clawAttackInfo, Vector3.up, Vector3.zero);
        
        else if(hitbox == tailHitbox)
        {
            Vector3 dir = (hurtbox.transform.position - transform.position).normalized;
            hurtbox.Hit(AIStateMachine.StealthAgent, tailAttackInfo, Vector3.up, dir * tailForce);
        }

        else if(hitbox == smashHitbox)
            hurtbox.Hit(AIStateMachine.StealthAgent, smashAttackInfo, Vector3.up, Vector3.up* smashForce);
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

    public StealthAgent GetEnemy()
    {
        return enemy;
    }
}
