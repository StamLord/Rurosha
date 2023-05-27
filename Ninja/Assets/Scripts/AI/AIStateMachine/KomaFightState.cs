using System.Collections;
using UnityEngine;

public class KomaFightState : FightAIState, IHitboxResponder
{
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

    private enum SubState {CIRCLE, MELEE}
    [SerializeField] private SubState subState;
    [SerializeField] private float subStateTime;
    private float enemyDistance;

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

    public void GetMessage(string message)
    {
        Debug.Log(gameObject.name + " Got message: " + message);
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

    private void Circle()
    {
        Vector3 target = enemy.transform.position; // Default to enemy position
        Vector3 dir = enemy.transform.position + Vector3.forward; // Vector to us from enemy

        currentCircleAngle += Time.deltaTime * circleSpeed; // Increment angle around enemy
        target += Quaternion.Euler(0, currentCircleAngle, 0) * Vector3.forward * circleRadius; // Target moves around the player on the y axis
        Debug.DrawLine(enemy.transform.position, target, Color.red);

        // Move to target
        MoveTo(target);

        if(Time.time - subStateTime > circleDuration)
            SwitchState(SubState.MELEE);
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
}
