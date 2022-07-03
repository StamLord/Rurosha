using UnityEngine;
using System.Collections;

public class FightAIState : AIState
{
    [Header("Movement")]
    [SerializeField] private float enemyDistance = 3f;
    [SerializeField] private float pathRecalculateDistance = 3f;
    [SerializeField] private float faceEnemyDistance = 5f;

    [Header("Attack")]
    [SerializeField] private bool defensive;
    [SerializeField] private float defenseRange = 5f;
    [Range(1,5)] [SerializeField] private int aggressive;
    [SerializeField] private float dontAttackInterval = 2f;
    [SerializeField] private bool attackCombo;
    [SerializeField] private int attackMaxCombo = 3;
    [SerializeField] private float attackInterval = 3f;

    private StealthAgent enemy;
    private Vector3 lastPathTarget;
    private Coroutine attackCoroutine;
    public bool canAttack = true;
    public bool defending;

    protected override void OnEnterState()
    {
        enemy = (AIStateMachine.enemy)? AIStateMachine.enemy : null;
        lastPathTarget = enemy.transform.position;
        MoveTo(enemy.transform.position);

        AIStateMachine.AwarenessAgent.OnLoseAgent += LoseAgent;

        // Draw weapon
        PressButton("2");
        canAttack = true;

        if(debug)
            AIStateMachine.SetDebugColor(Color.red);
    }

    public override void OnStateUpdate()
    {
        // If close enough, AI faces player
        if(TooFar(enemy.transform.position, faceEnemyDistance))
            LookTowards(AIStateMachine.GetNextPosition());
        else
            LookTowards(enemy.transform.position);

        // Move to player when distance between AI and player too big and when player moved enough from his last position we calculated path
        bool inAtkRange = !TooFar(enemy.transform.position, enemyDistance);
        if(inAtkRange == false &&
            Vector3.Distance(enemy.transform.position, lastPathTarget) > pathRecalculateDistance)
        {
            lastPathTarget = enemy.transform.position;
            MoveTo(enemy.transform.position);
        }
        
        if (inAtkRange &&   // Attack if in range and 
            canAttack)      // Not already attacking
        {
            // Aggressive chance roll
            int agg = Random.Range(0, aggressive); // Returns 0 .. 4, at aggressive = 1 we never attack, at aggressive = 5 we attack 4/5
            if(agg == 0)
                attackCoroutine = StartCoroutine("DontAttack", dontAttackInterval);
            else
            {            
                StopDefend();
                attackCoroutine = StartCoroutine("Attack", attackInterval);
                return;
            }
        }
        
        if(defensive)
        {
            bool inDefRange = Vector3.Distance(transform.position, enemy.transform.position) < defenseRange;
            if(inDefRange)
                StartDefend();
            else
                StopDefend();
        }
    }

    private void StartDefend()
    {
        if(defending == true) return;
        HoldButton("defense");
        defending = true;
    }

    private void StopDefend()
    {
        if(defending == false) return;
        StopHoldButton("defense");
        defending = false;
    }

    protected override void OnExitState()
    {
        AIStateMachine.AwarenessAgent.OnLoseAgent -= LoseAgent;
    }

    private void LoseAgent(StealthAgent agent)
    {
        // Ignore any agent except our enemy
        if(agent != enemy) return;

        // Start searching
        AIStateMachine.enemyLastSeen = agent.transform.position; // Last position
        Rigidbody rb = agent.GetComponent<Rigidbody>();
        AIStateMachine.enemyLastDir = (rb)? rb.velocity : Vector3.zero; // Last direction
        AIStateMachine.enemyLastDir.y = 0; // Flatten direction to discard jumping / falling vectors

        // Switch to SearchAIState
        AIStateMachine.SwitchState(2);
    }

    private IEnumerator Attack(float waitAfterAttack)
    {
        canAttack = false;
        int attacks = (attackCombo)? attackMaxCombo : 1;

        for (var i = 0; i < attacks; i++)
        {
            PressButton("MB1");
            //MoveTo(enemy.transform.position);
            if(i < attacks - 1)
                yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(waitAfterAttack);
        canAttack = true;
    }

    private IEnumerator DontAttack(float wait)
    {
        canAttack = false;
        yield return new WaitForSeconds(wait);
        canAttack = true;
    }
    
    private void OnDrawGizmos() 
    {
        if(debug == false || enemy == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(enemy.transform.position + Vector3.up * .5f, .5f);
    }
}
