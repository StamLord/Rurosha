using System.Collections;
using System;
using UnityEngine;

public class KomaMagicFightState : FightAIState, IHitboxResponder
{
    private enum SubState { CIRCLE, 
                            CAST_HEAL, 
                            CAST_BEAM, 
                            CAST_BALL,
                            CAST_BURST}

    [Header ("Ally")]
    [SerializeField] private KomaPhysicalFightState ally;
    [SerializeField] private CharacterStats allyStats;

    [Header ("Circle")]
    [SerializeField] private float circleDuration = 5;
    [SerializeField] private float circleRadius = 5;
    [SerializeField] private float circleSpeed = 5;
    [SerializeField] private float currentCircleAngle = 0;

    [Header ("Casting Ball")]
    [SerializeField] private AttackInfo ballAttackInfo = new AttackInfo(30, 15);
    [SerializeField] private Hitbox ballHitbox;
    [SerializeField] private float ballRigidForce = 36f;
    [SerializeField] private ParticleSystem ballBurstVFX;
    [SerializeField] private float castBallDuration = 3f;
    [SerializeField] private int ballsPerCycle = 3;
    [SerializeField] private GameObject visualCastBallObject;
    [SerializeField] private GameObject ballProjectile;
    [SerializeField] private Transform projectileOrigin;

    [Header ("Casting Beam")]
    [SerializeField] private AttackInfo beamAttackInfo = new AttackInfo(30, 15);
    [SerializeField] private Hitbox beamHitbox;
    [SerializeField] private float beamRigidForce = 24f;
    [SerializeField] private float castBeamDuration = 3f;
    [SerializeField] private float castBeamRotateSpeed = 5f;
    [SerializeField] private float holdBeamDuration = 3f;
    [SerializeField] private float beamSpeed = 5f;
    [SerializeField] private float beamHitboxResetsEvery = .5f;
    [SerializeField] private ParticleSystem beamChargeVfx;
    [SerializeField] private ParticleSystem beamReadyVfx;
    [SerializeField] private GameObject beamEndVfx;
    [SerializeField] private LineRenderer beamRenderer;
    [SerializeField] private Transform beamOrigin;

    [Header ("Casting Heal")]
    [SerializeField] private AttackInfo healInfo = new AttackInfo(100, 150);
    [SerializeField] private float castHealDuration = 3f;
    [SerializeField] private ParticleSystem castHealVfx;
    [SerializeField] private ParticleSystem targetHealVfx;

    [Header ("AOE Burst")]
    [SerializeField] private AttackInfo burstAttackInfo = new AttackInfo(30, 15);
    [SerializeField] private Hitbox burstHitbox;
    [SerializeField] private float burstRigidForce = 36f;
    [SerializeField] private ParticleSystem burstVFX;
    [SerializeField] private float burstRange = 4f;
    [SerializeField] private float burstActiveDuration = 1f;
    [SerializeField] private float burstCooldown = 5f;

    [Header ("SubState")]
    [SerializeField] private SubState subState;
    [SerializeField] private float subStateTime;

    private float enemyDistance;
    private bool isCasting;
    private Coroutine castingCoroutine;
    private int castBallCycle = 0;
    private float lastBurstTime;
    private GameObject ballProj;
    [SerializeField] private bool shouldHealAlly;
    [SerializeField] private bool isAllyDead;

    private SubState lastSubState;

    private void Start()
    {
        ballHitbox?.SetResponder(this, transform.root);
        beamHitbox?.SetResponder(this, transform.root);
        burstHitbox?.SetResponder(this, transform.root);
    }

    protected override void OnEnterState()
    {
        canAttack = true;

        // Sync enemy with ally
        if(ally)
            enemy = ally.GetEnemy() ?? AIStateMachine.enemy ;
        else
            enemy = AIStateMachine.enemy;

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
            case SubState.CAST_HEAL:
                CastHeal();
                break;
            case SubState.CAST_BALL:
                CastBall();
                break;
            case SubState.CAST_BEAM:
                CastBeam();
                break;
            case SubState.CAST_BURST:
                CastBurst();
                break;
        }
    }

    protected override void OnExitState()
    {
        if(SquadAgent != null) SquadAgent.OnGetMessage -= GetMessage;
    }

    private void SwitchState(SubState state)
    {
        lastSubState = subState;
        subState = state;
        subStateTime = Time.time;
    }
    
    public void GetMessage(string message, SquadAgent sender)
    {
        KomaPhysicalFightState.MessageType messageType;
        bool success = Enum.TryParse<KomaPhysicalFightState.MessageType>(message, true, out messageType);

        if(success == false) return;

        switch(messageType)
        {
            case KomaPhysicalFightState.MessageType.LOW_HEALTH:
                shouldHealAlly = true;
                break;
            case KomaPhysicalFightState.MessageType.ALLY_DEATH:
                isAllyDead = true;
                break;
        }
    }

    public void CollisionWith(Collider collider, Hitbox hitbox)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();

        if(hurtbox)
        {
            AttackInfo attackInfo;

            if(hitbox == ballHitbox)
                attackInfo = ballAttackInfo;
            else if(hitbox == burstHitbox)
                attackInfo = burstAttackInfo;
            else if(hitbox == beamHitbox)
                attackInfo = beamAttackInfo;
            else
                attackInfo = new AttackInfo();
            
            hurtbox.Hit(AIStateMachine.StealthAgent, attackInfo, Vector3.zero, Vector3.up);
        }

        Rigidbody rb = collider.transform.parent.GetComponent<Rigidbody>();

        if(rb)
        {
            if(hitbox == ballHitbox)
                rb.AddForce((collider.transform.position - ballProj.transform.position).normalized * ballRigidForce, ForceMode.Impulse);
            else if(hitbox == beamHitbox)
                rb.AddForce((collider.transform.position - beamOrigin.position).normalized * beamRigidForce, ForceMode.Impulse);
            else if (hitbox == burstHitbox)
                rb.AddForce((collider.transform.position - transform.position).normalized * ballRigidForce, ForceMode.Impulse);
        }
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

    private void Circle()
    {
        Vector3 target = enemy.transform.position; // Default to enemy position

        //currentCircleAngle += Time.deltaTime * circleSpeed; // Increment angle around enemy
        //currentCircleAngle %= 360f; // Reset at 360

        // Target moves around the player on the y axis
        target += Quaternion.Euler(0, currentCircleAngle, 0) * Vector3.right * circleRadius;
        Debug.DrawLine(enemy.transform.position, target, Color.blue);

        // Move to target
        MoveTo(target);

        if(Time.time - subStateTime > circleDuration)
            SwitchState(SubState.CAST_BALL);
    }

    private void CastHeal()
    {
        if(isCasting)
            return;

        // Start casting
        castingCoroutine = StartCoroutine("HealCoroutine");
    }

    private void CastBall()
    {
        // Heal ally
        if(shouldHealAlly)
        {
            SwitchState(SubState.CAST_HEAL);
            return;
        }

        // Finished with balls, move on to beams
        if(castBallCycle >= ballsPerCycle)
        {
            SwitchState(SubState.CAST_BEAM);
            ally.StartMelee();
            return;
        }

        if(isCasting)
            return;

        // Start casting
        castingCoroutine = StartCoroutine("BallCoroutine");
    }

    private void CastBeam()
    {
        // Heal ally
        if(shouldHealAlly)
        {
            SwitchState(SubState.CAST_HEAL);
            return;
        }

        if(isCasting)
            return;

        // Start casting
        castingCoroutine = StartCoroutine("BeamCoroutine");
    }

    private void CastBurst()
    {
        if(isCasting)
            return;

        // Start casting
        castingCoroutine = StartCoroutine("BurstCoroutine");
    }

    private IEnumerator HealCoroutine()
    {
        isCasting = true;
        
        // Stop moving
        MoveStop();

        if(castHealVfx)
            castHealVfx.Play();
        
        Animator?.Play("cast_start");

        // Casting time
        float startTime = Time.time;
        while(Time.time - startTime <= castHealDuration)
        {
            LookTowards(ally.transform.position);
            yield return null;
        }

        if(castHealVfx)
            castHealVfx.Stop();
        
        Animator?.Play("cast_end");

        // Heal Ally
        if(allyStats) allyStats.RestoreHealth(healInfo.softDamage, healInfo.hardDamage);

        if(targetHealVfx)
        {   
            targetHealVfx.transform.position = ally.transform.position;
            targetHealVfx.Play();
        }

        // Give casting animation time to finish
        yield return new WaitForSeconds(1f);
        
        shouldHealAlly = false;
        isCasting = false;

        // Switch back to previous action
        SwitchState(lastSubState);
    }

    private IEnumerator BallCoroutine()
    {
        isCasting = true;
        
        // Stop moving
        MoveStop();

        Animator?.Play("cast_ball");
        visualCastBallObject?.SetActive(true);

        // Ally starts waiting for ball cast
        if(ally) 
            ally.StartBallWait();

        // Casting time
        float startTime = Time.time;
        while(Time.time - startTime <= castBallDuration)
        {
            // Rotate to face ally
            if(ally)
                LookTowards(ally.transform.position, 5f);

            // AOE Burst if enemy too close
            /*
            if(ValidateBurst())
            {
                visualCastBallObject?.SetActive(false);
                isCasting = false;

                Animator?.Play("idle");
                SwitchState(SubState.CAST_BURST);
                yield break;
            }*/

            visualCastBallObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (Time.time - startTime) / castBallDuration);
            yield return null;
        }

        Animator?.Play("idle");
        visualCastBallObject?.SetActive(false);

        // Creates projectile
        if(ballProj == null)
            ballProj = CreateBallProjectile();
        else // Reset
            ballProj.transform.position = (projectileOrigin)? projectileOrigin.position : transform.position + Vector3.up; 
        
        // Ally waikts for ball to bounce back
        if(ally)
            ally.StartBallReady();

        // Track ball
        Vector3 startPos = ballProj.transform.position;
        Vector3 targetPos = (ally)? ally.transform.position : enemy.transform.position; // Either ally if one exists (to bounce off) or the enemy
        Vector3 targetDir = targetPos - startPos;
        float travelDistance = targetDir.magnitude;
        float travelTime = travelDistance / 20f; // Speed 20
        startTime = Time.time;
        Vector3 endPos = targetPos + Vector3.up;

        ballProj.transform.forward = targetDir;
        ballProj.SetActive(true);

        while(Time.time - startTime <= travelTime)
        {
            float ballEnemyDistance = Vector3.Distance(enemy.transform.position, ballProj.transform.position);

            // Move ball
            ballProj.transform.position = Vector3.Lerp(startPos, endPos, (Time.time - startTime ) / travelTime);
            
            // Ball hits enemy
            if(ballEnemyDistance <= 2f)
            {
                // Boom
                BallBurst(ballProj.transform.position);
                break;
            }

            if(ally)
            {
                float ballAllyDistance = Vector3.Distance(ally.transform.position, ballProj.transform.position);
                
                // Ally animation
                if(ballAllyDistance <= 4f)
                    ally.BallBounceAnimation();
            
                // Bounce ball back from ally
                if(ballAllyDistance <= 3f)
                {
                    Vector3 dir = enemy.transform.position - ally.transform.position;
                    ballProj.transform.forward = dir;

                    // Restart while conditions
                    travelDistance = Vector3.Distance(ballProj.transform.position, enemy.transform.position);
                    travelTime = travelDistance / 20f; // Speed 20
                    startTime = Time.time;
                    startPos = ballProj.transform.position;
                    endPos = enemy.transform.position + Vector3.up;
                }
            }

            yield return null;
        }

        // Ball explodes at end of flight
        BallBurst(ballProj.transform.position);

        // Deactivate ball projectile
        ballProj.SetActive(false);
        castBallCycle++;
        isCasting = false;
    }

    private IEnumerator BeamCoroutine()
    {
        isCasting = true;
        SquadAgent.SendMessage("Started casting");
        
        // Stop moving
        MoveStop();

        Animator?.Play("cast_beam_start");
        
        if(beamChargeVfx)
            beamChargeVfx.Play();

        // Casting time
        float startTime = Time.time;
        while(Time.time - startTime <= castBeamDuration)
        {
            float p = (Time.time - startTime) / castBeamDuration;
            
            if(p >= .5f)
            {
                if(beamChargeVfx && beamChargeVfx.isPlaying)
                    beamChargeVfx.Stop();

                if(beamReadyVfx && beamReadyVfx.isPlaying == false)
                    beamReadyVfx.Play();

                // AOE Burst if enemy too close
                if(ValidateBurst())
                {
                    isCasting = false;
                    Animator?.Play("idle");
                    if(beamReadyVfx) beamReadyVfx.Stop();

                    SwitchState(SubState.CAST_BURST);
                    yield break;
                }
            }

            // Rotate to face enemy
            LookTowards(enemy.transform.position, castBeamRotateSpeed);
            
            yield return null;
        }

        if(beamReadyVfx)
            beamReadyVfx.Stop();

        Animator?.Play("cast_beam_mid");
        
        beamRenderer.enabled = true;
        UpdateBeamRenderer(beamOrigin.position, enemy.transform.position);
        beamEndVfx.SetActive(true);

        // Beam time
        startTime = Time.time;
        
        Vector3 projectedPos = enemy.transform.position;    // Current position of the beam end assuming nothing blocks it
        Vector3 targetPos;                                  // Position we try to get to - enemy
        Vector3 correctionDir;                              // Direction from projected to target. Used to move beam slowly to target
        Vector3 toTarget;                                   // Direction from beam start to projected. Used to raycast
        RaycastHit hit;                                     // Hit info from raycast if anything blocks beam
        Vector3 finalPos;                                   // Final position of the beam end
        
        float lastHitboxReset = startTime;                  // Used to reset collider everty second so it can damage over time

        beamHitbox.StartColliding();

        while(Time.time - startTime <= holdBeamDuration)
        {
            // Rotate to face enemy
            LookTowards(enemy.transform.position, beamSpeed);

            // Chase enemy with beam
            targetPos = enemy.transform.position + Vector3.up;

            // If outside of range, we aim for the floor and clamp to max distance
            // if(enemyDistance > 15f)
            //     targetPos = beamOrigin.position + Vector3.ClampMagnitude(enemy.transform.position - beamOrigin.position, 15f);
            
            correctionDir = (targetPos - projectedPos).normalized;
            projectedPos = projectedPos + correctionDir * beamSpeed * Time.deltaTime;

            // Check what we are hitting
            toTarget = projectedPos - beamOrigin.position;
            finalPos = projectedPos;

            if(Physics.Raycast(beamOrigin.position, toTarget, out hit, toTarget.magnitude))
                finalPos = hit.point;
            
            UpdateBeamRenderer(beamOrigin.position, finalPos);
            beamEndVfx.transform.position = finalPos;
            beamHitbox.transform.position = finalPos;

            if(Time.time - lastHitboxReset >= beamHitboxResetsEvery)
            {
                lastHitboxReset = Time.time;
                beamHitbox.ForgetCollided();
            }

            yield return null;
        }

        Animator?.Play("idle");
        beamRenderer.enabled = false;
        beamHitbox.StopColliding();
        beamHitbox.ForgetCollided();
        beamEndVfx.SetActive(false);

        // Give casting animation time to finish
        yield return new WaitForSeconds(1f);

        isCasting = false;
    }

    private IEnumerator BurstCoroutine()
    {
        isCasting = true;
        lastBurstTime = Time.time;
        burstVFX.Play();
        burstHitbox.isActive = true;

        yield return new WaitForSeconds(burstActiveDuration);

        burstHitbox.isActive = false;
        isCasting = false;

        // Switch back to previous action
        SwitchState(lastSubState);
    }

    private bool ValidateBurst()
    {
        if(Time.time - lastBurstTime < burstCooldown ||
            enemyDistance > burstRange)
            return false;

        return true;
    }

    private void UpdateBeamRenderer(Vector3 pos1, Vector3 pos2)
    {
        if(beamRenderer == null) return;
        
        beamRenderer.SetPosition(0, pos1);
        beamRenderer.SetPosition(1, pos2);
    }

    private void BallBurst(Vector3 position)
    {
        ballHitbox.transform.position = position;
        ballHitbox.StartColliding(true);
        ballBurstVFX.Play();
    }

    private IEnumerator Cooldown(float cooldown)
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }

    private GameObject CreateBallProjectile()
    {
        // Origin of proejctile, either a reference or 1 unit from our origin (floor)
        Vector3 origin = (projectileOrigin)? projectileOrigin.position : transform.position + Vector3.up;

        // Instantiate ball object
        GameObject ballInstance = Instantiate(ballProjectile, origin, Quaternion.identity);

        // Get hitbox
        Hitbox hitbox = ballInstance.GetComponent<Hitbox>();
        hitbox?.SetResponder(this, transform.root);

        return ballInstance;
    }

    public StealthAgent GetEnemy()
    {
        return enemy;
    }
}
