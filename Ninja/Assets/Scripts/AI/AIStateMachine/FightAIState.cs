using UnityEngine;

public class FightAIState : AIState
{
    [SerializeField] private float enemyDistance = 1f;

    private StealthAgent enemy;

    protected override void OnEnterState()
    {
        enemy = (AIStateMachine.enemy)? AIStateMachine.enemy : null;
        AIStateMachine.AwarenessAgent.OnLoseAgent += LoseAgent;

        if(debug)
            AIStateMachine.SetDebugColor(Color.red);
    }

    public override void OnStateUpdate()
    {
        LookTowards(enemy.transform.position);

        if(TooFar(enemy.transform.position, enemyDistance))
            MoveTo(enemy.transform.position);
        else
            MoveStop();
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
    
    private void OnDrawGizmos() 
    {
        if(debug == false) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(enemy.transform.position + Vector3.up * .5f, .5f);
    }
}
