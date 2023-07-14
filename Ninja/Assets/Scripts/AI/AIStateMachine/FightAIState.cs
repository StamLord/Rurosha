using UnityEngine;

public class FightAIState : AIState
{
    [SerializeField] protected float pathRecalculateDistance = 3f;
    [SerializeField] protected float awarenessCheatRange = 3f;

    protected StealthAgent enemy;
    protected Coroutine attackCoroutine;
    public bool canAttack = false;

    protected override void OnEnterState()
    {
        enemy = (AIStateMachine.enemy)? AIStateMachine.enemy : null;
        AIStateMachine.AwarenessAgent.OnLoseAgent += LoseAgent;
    }

    public override void OnStateUpdate()
    {
       
    }

    protected override void OnExitState()
    {
        AIStateMachine.AwarenessAgent.OnLoseAgent -= LoseAgent;
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

    private void OnDrawGizmos() 
    {
        if(debug == false || enemy == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(enemy.transform.position + Vector3.up * .5f, .5f);
    }
}
