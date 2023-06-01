using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KomaFightState : FightAIState, IHitboxResponder
{
    private enum KomainuType {PHYSICAL, MAGIC}
    private enum SubState { CIRCLE, 
                            MELEE,
                            BALL_READY, 
                            CAST_HEAL, 
                            CAST_BEAM, 
                            CAST_BALL}

    [Header ("Komainu Type")]
    [SerializeField] private KomainuType komainuType;

    [Header ("Hitbox")]
    [SerializeField] private Hitbox[] hitbox;

    [Header ("Melee")]
    [SerializeField] private float meleeDuration = 10;
    [SerializeField] private AttackInfo clawAttackInfo = new AttackInfo(7, 4, DamageType.Slash);
    [SerializeField] private float clawAttackRange = 2;
    [SerializeField] private float clawAttackCooldown = 1;
    [SerializeField] private float minimumTimeInClawAttack = 3;

    [Header ("Circle")]
    [SerializeField] private float circleDuration = 5;
    [SerializeField] private float circleRadius = 5;
    [SerializeField] private float circleSpeed = 5;
    [SerializeField] private float currentCircleAngle = 0;

    [Header ("Casting")]
    [SerializeField] private float castHealDuration = 3f;
    [SerializeField] private float castBallDuration = 3f;
    [SerializeField] private float castBeamDuration = 3f;
    [SerializeField] private float holdBeamDuration = 3f;
    [SerializeField] private GameObject visualCastObject;
    [SerializeField] private GameObject ballProjectile;
    [SerializeField] private Transform projectileOrigin;
    [SerializeField] private GameObject beamObject;

    [Header ("SubState")]
    [SerializeField] private SubState subState;
    [SerializeField] private float subStateTime;

    private float enemyDistance;
    private Transform ally;
    private bool isCasting;
    private Coroutine castingCoroutine;

    private void Start()
    {
        foreach(Hitbox h in hitbox)
        {
            h.SetResponder(this);
            h.SetIgnoreTransform(transform.root);
        }
    }

    protected override void OnEnterState()
    {
        canAttack = true;
        enemy = (AIStateMachine.enemy)? AIStateMachine.enemy : null;
        Stats.OnHit += Hit;
        //AIStateMachine.AwarenessAgent.OnLoseAgent += LoseAgent;
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
            case SubState.BALL_READY:
                BallReady();
                break;
            case SubState.CAST_HEAL:
                CastHeal();
                break;
            case SubState.CAST_BALL:
                CastBall();
                break;
            case SubState.CAST_BEAM:
                CastBeam();
                break;
        }
    }

    protected override void OnExitState()
    {
        if(SquadAgent != null) SquadAgent.OnGetMessage -= GetMessage;
    }

    private void SwitchState(SubState state)
    {
        subState = state;
        subStateTime = Time.time;
    }

    public void GetMessage(string message, SquadAgent sender)
    {
        // All messages are only listened to if we are not doing anything else
        if(subState != SubState.CIRCLE && subState != SubState.MELEE)
            return;

        switch(message)
        {
            case "Low Health":
                if(komainuType == KomainuType.MAGIC)
                {
                    ally = sender.GetTransform();
                    SwitchState(SubState.CAST_HEAL);
                }
                break;
            case "Ball Cast Start":
                if(komainuType == KomainuType.PHYSICAL)
                {
                    ally = sender.GetTransform();
                    SwitchState(SubState.BALL_READY);
                }
                break;
        }
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();

        if(hurtbox)
            hurtbox.Hit(AIStateMachine.StealthAgent, clawAttackInfo.softDamage, clawAttackInfo.hardDamage, Vector3.up, clawAttackInfo.damageType, clawAttackInfo.statuses);
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

        if(Time.time - subStateTime > meleeDuration)
            SwitchState(SubState.CIRCLE);
    }

    private void BallReady()
    {
        // Rotate to face ally
        if(ally)
            LookTowards(ally.position);
        
        // We  stand behind enemy to throw back the ball

        Vector3 target = enemy.transform.position; // Default to enemy position
        Vector3 dir = enemy.transform.position - ally.transform.position; // Vector from ally to enemy
        target += dir;

        // Move to target
        MoveTo(target);

    }

    private void Circle()
    {
        Vector3 target = enemy.transform.position; // Default to enemy position

        //currentCircleAngle += Time.deltaTime * circleSpeed; // Increment angle around enemy
        //currentCircleAngle %= 360f; // Reset at 360

        // Target moves around the player on the y axis
        switch(komainuType)
        {
            case KomainuType.PHYSICAL:
                target += Quaternion.Euler(0, currentCircleAngle, 0) * Vector3.left * circleRadius;
                Debug.DrawLine(enemy.transform.position, target, Color.red);
                break;
            case KomainuType.MAGIC:
                target += Quaternion.Euler(0, currentCircleAngle, 0) * Vector3.right * circleRadius;
                Debug.DrawLine(enemy.transform.position, target, Color.blue);
                break;
        }

        // Move to target
        MoveTo(target);

        if(Time.time - subStateTime > circleDuration)
        {
            switch(komainuType)
            {
                case KomainuType.PHYSICAL:
                    SwitchState(SubState.MELEE);
                    break;
                case KomainuType.MAGIC:
                    SwitchState(SubState.CAST_BALL);
                    break;
            }
        }
    }

    private void CastHeal()
    {
        // Rotate to face ally
        LookTowards(ally.position);

        if(isCasting)
            return;

        // Start casting
        castingCoroutine = StartCoroutine("HealCoroutine");
    }

    private void CastBall()
    {
        // Rotate to face ally
        if(ally)
            LookTowards(ally.position);

        if(isCasting)
            return;

        // Start casting
        castingCoroutine = StartCoroutine("BallCoroutine");
    }

    private void CastBeam()
    {
        // Rotate to face enemy
        LookTowards(enemy.transform.position);

        if(isCasting)
            return;

        // Start casting
        castingCoroutine = StartCoroutine("BeamCoroutine");
    }

    private IEnumerator HealCoroutine()
    {
        isCasting = true;
        
        // Stop moving
        MoveStop();

        Animator?.Play("cast_start");
        visualCastObject?.SetActive(true);

        // Casting time
        float startTime = Time.time;
        while(Time.time - startTime <= castHealDuration)
        {
            visualCastObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (Time.time - startTime) / castHealDuration);
            yield return null;
        }

        Animator?.Play("cast_end");
        visualCastObject?.SetActive(false);

        // Heal Ally
        CharacterStats stats = ally.root.GetComponent<CharacterStats>();
        if(stats) stats.RestoreHealth(30, 50);

        // Give casting animation time to finish
        yield return new WaitForSeconds(1f);

        isCasting = false;
    }

    private IEnumerator BallCoroutine()
    {
        isCasting = true;
        SquadAgent.SendMessage("Ball Cast Start");
        
        // Stop moving
        MoveStop();

        Animator?.Play("cast_start");
        visualCastObject?.SetActive(true);

        // Casting time
        float startTime = Time.time;
        while(Time.time - startTime <= castHealDuration)
        {
            visualCastObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (Time.time - startTime) / castHealDuration);
            yield return null;
        }

        Animator?.Play("cast_end");
        visualCastObject?.SetActive(false);

        NewMessage("Take that!", 2f);

        // Creates projectile
        CastProjectile();

        // Give casting animation time to finish
        yield return new WaitForSeconds(1f);

        isCasting = false;
    }

    private IEnumerator BeamCoroutine()
    {
        isCasting = true;
        SquadAgent.SendMessage("Started casting");
        
        // Stop moving
        MoveStop();

        Animator?.Play("cast_beam_start");
        visualCastObject?.SetActive(true);

        // Casting time
        float startTime = Time.time;
        while(Time.time - startTime <= castHealDuration)
        {
            visualCastObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (Time.time - startTime) / castHealDuration);
            yield return null;
        }

        visualCastObject?.SetActive(false);

        Animator?.Play("cast_beam_mid");

        yield return new WaitForSeconds(holdBeamDuration);

        Animator?.Play("cast_beam_end");

        // Give casting animation time to finish
        yield return new WaitForSeconds(1f);

        isCasting = false;
    }

    private bool CanAttack()
    {
        // Allow for .2 unit deviation in case we stopped short of melee range
        return canAttack && enemyDistance < clawAttackRange + .2f;
    }

    private void AttemptMeleeAttack()
    {
        if(CanAttack() == false) return;
        
        Animator?.Play("claw_attack");
        StartCoroutine(Cooldown(clawAttackCooldown));
    }

    private IEnumerator Cooldown(float cooldown)
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }

    private void Hit(int softDamage, int hardDamage)
    {
        if(Stats.Health < 50)
            SendMessage("Low Health");
    }

    private void CastProjectile()
    {
        // Origin of proejctile, either a reference or 1 unit from our origin (floor)
        Vector3 origin = (projectileOrigin)? projectileOrigin.position : transform.position + Vector3.up;

        // Instantiate and orient projectile object
        Vector3 target = enemy.transform.position;
        // Direction to target with offset of 1.5 unit up
        Vector3 dir = (target + Vector3.up * 1.5f - origin).normalized;

        GameObject go = Instantiate(ballProjectile, origin, Quaternion.LookRotation(dir, Vector3.up));
        Projectile proj = go.GetComponent<Projectile>();

        // Setup projectile
        if(proj)
        {
            proj.SetOwner(StealthAgent);
            proj.SetIgnoreTransform(transform.root);
        }
    }
}
