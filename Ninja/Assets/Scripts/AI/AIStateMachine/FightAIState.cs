using UnityEngine;
using System.Collections;

public class FightAIState : AIState
{
    [Header("Movement")]
    [SerializeField] protected float circleRange = 5f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float pathRecalculateDistance = 3f;
    [SerializeField] protected float faceEnemyDistance = 5f;

    [Header("Attack")]
    [SerializeField] protected bool defensive;
    [SerializeField] protected float defenseRange = 5f;
    [Range(1,5)] [SerializeField] protected int aggressive;
    [SerializeField] protected float dontAttackInterval = 2f;
    [SerializeField] protected bool attackCombo;
    [SerializeField] protected int attackMaxCombo = 3;
    [SerializeField] protected float attackInterval = 3f;

    [Header("Awareness Cheat Range")]
    [SerializeField] protected float awarenessCheatRange = 3f;

    protected StealthAgent enemy;
    protected Vector3 lastPathTarget;
    protected Coroutine attackCoroutine;
    public bool canAttack = false;
    public bool canAdvance = false;
    public bool defending;

    private TargetManager targetManager;

    protected override void OnEnterState()
    {
        enemy = (AIStateMachine.enemy)? AIStateMachine.enemy : null;

        // Register to target manager
        if(enemy)
        {
            targetManager = enemy.GetComponent<TargetManager>();
            if(targetManager)
            {
                targetManager.AddFighter(this);
                Stats.OnDeath += OnDeath;
            }
            else
            {
                canAttack = true;
                canAdvance = true;
            }
        }
        
        lastPathTarget = enemy.transform.position;
        MoveTo(enemy.transform.position);

        AIStateMachine.AwarenessAgent.OnLoseAgent += LoseAgent;

        // Draw weapon
        DrawWeapon();
    }

    public override void OnStateUpdate()
    {
        // If close enough, AI faces player
        if(TooFar(enemy.transform.position, faceEnemyDistance))
            LookTowards(AIStateMachine.GetNextPosition());
        else
            LookTowards(enemy.transform.position);

        // Move to player when distance between AI and player too big and when player moved enough from his last position we calculated path
        
        Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy
        Vector3 circleTarget = enemy.transform.position + dir * circleRange; // Get point at needed distance from enemy
        Vector3 attackTarget = enemy.transform.position + dir * attackRange; // Get point at needed distance from enemy
        Vector3 target = (canAdvance)? attackTarget : circleTarget; // Relevant target for us

        float distaneFromTarget = Vector3.Distance(transform.position, target); // Our distance from our target position (circle or attack range)
        bool inRange = distaneFromTarget <= .3f; // We allow for .3f to be the threshold to account for inconsistency in moving

        // Recalculate if too far or target position changed enough
        if(inRange == false || Vector3.Distance(target, lastPathTarget) > pathRecalculateDistance)
        {
            lastPathTarget = target;
            MoveTo(target);
        }
        
        if (canAdvance && inRange && canAttack) // Close enough to attack range and not already attacking
            Attack();
        
        if(defensive)
        {
            bool inDefRange = Vector3.Distance(transform.position, enemy.transform.position) < defenseRange;
            if(inDefRange)
                StartDefend();
            else
                StopDefend();
        }
    }

    protected virtual void Attack()
    {
        // Aggressive chance roll
        int agg = Random.Range(0, aggressive); // Returns 0 .. 4, at aggressive = 1 we never attack, at aggressive = 5 we attack 4/5
        
        if(agg == 0)
            attackCoroutine = StartCoroutine("DontAttack", dontAttackInterval);
        else
        {            
            StopDefend();
            attackCoroutine = StartCoroutine("AttackCoroutine", attackInterval);
            return;
        }
    }
    
    protected virtual void DrawWeapon()
    {
        PressButton("2");
    }

    protected void StartDefend()
    {
        if(defending == true) return;
        HoldButton("defense");
        defending = true;
    }

    protected void StopDefend()
    {
        if(defending == false) return;
        StopHoldButton("defense");
        defending = false;
    }

    protected override void OnExitState()
    {
        AIStateMachine.AwarenessAgent.OnLoseAgent -= LoseAgent;
        Stats.OnDeath -= OnDeath;
        
        if(targetManager)
            targetManager.RemoveFighter(this);
    }

    protected virtual void LoseAgent(StealthAgent agent)
    {
        // Ignore any agent except our enemy
        if(agent != enemy) return;

        // Start searching
        AIStateMachine.enemyLastSeen = agent.transform.position; // Last position
        Rigidbody rb = agent.GetComponent<Rigidbody>();
        AIStateMachine.enemyLastDir = (rb)? rb.velocity : Vector3.zero; // Last direction
        AIStateMachine.enemyLastDir.y = 0; // Flatten direction to discard jumping / falling vectors

        // Switch to SearchAIState
        SwitchState(AIStateMachine.StateName.SEARCH);
    }

    private IEnumerator AttackCoroutine(float waitAfterAttack)
    {
        canAttack = false;
        int attacks = (attackCombo)? attackMaxCombo : 1;

        for (var i = 0; i < attacks; i++)
        {
            PressButton("MB1");
            if(i < attacks - 1)
                yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(waitAfterAttack);

        if(targetManager)
            targetManager.FinishedAttack(this);
        else
            canAttack = true;
    }

    private IEnumerator DontAttack(float wait)
    {
        canAttack = false;
        yield return new WaitForSeconds(wait);
        canAttack = true;
    }

    public void AllowAttack(bool allow)
    {
        canAttack = allow;
    }

    public void AllowAdvance(bool allow)
    {
        canAdvance = allow;
    }
    
    private void OnDrawGizmos() 
    {
        if(debug == false || enemy == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(enemy.transform.position + Vector3.up * .5f, .5f);
    }

    private void OnDeath()
    {
        targetManager.RemoveFighter(this);
    }
}
