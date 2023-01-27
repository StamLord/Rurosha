using UnityEngine;
using System.Collections;

public class FleeAIState : AIState
{
    [Header("Movement")]
    [SerializeField] protected float pathRecalculateDistance = 3f;
    [SerializeField] protected float fleeDistance = 20f;
    [SerializeField] protected float secondsBeforeIdle = 10f;

    protected StealthAgent enemy;
    protected Vector3 lastFarthestPoint;
    protected Coroutine returnToIdleTimer;

    protected override void OnEnterState()
    {
        enemy = (AIStateMachine.enemy)? AIStateMachine.enemy : null;
        
        lastFarthestPoint = enemy.transform.position;
        MoveTo(enemy.transform.position);

        AIStateMachine.AwarenessAgent.OnLoseAgent += LoseAgent;
        AIStateMachine.AwarenessAgent.OnSeeAgent += SeeAgent;
        
        AIStateMachine.HoldButton("run");
    }

    public override void OnStateUpdate()
    {
        // Turn towards next position along path
        LookTowards(AIStateMachine.GetNextPosition());

        // Move to player when distance between AI and player too big and when player moved enough from his last position we calculated path
        Vector3 dir = (transform.position - enemy.transform.position).normalized; // Vector to us from enemy
        Vector3 farthestPoint = enemy.transform.position + dir * fleeDistance; // Get point at needed distance from enemy

        float distaneFromEnemy = Vector3.Distance(transform.position, enemy.transform.position);

        // Recalculate if target position changed enough
        if(Vector3.Distance(farthestPoint, lastFarthestPoint) > pathRecalculateDistance)
        {
            lastFarthestPoint = farthestPoint;
            MoveTo(farthestPoint);
        }
    }

    protected override void OnExitState()
    {
        AIStateMachine.StopHoldButton("run");
        AIStateMachine.AwarenessAgent.OnLoseAgent -= LoseAgent;
    }

    protected virtual void LoseAgent(StealthAgent agent)
    {
        // Ignore any agent except our enemy
        if(agent != enemy) return;

        // Return to idle after X time
        returnToIdleTimer = StartCoroutine("ReturnToIdleTimer");
    }

    protected virtual void SeeAgent(StealthAgent agent)
    {
        // Ignore any agent except our enemy
        if(agent != enemy) return;

        // Stop countdown
        if(returnToIdleTimer != null)
        {
            StopCoroutine("ReturnToIdleTimer");
            returnToIdleTimer = null;
        }
    }

    private IEnumerator ReturnToIdleTimer()
    {
        float startTime = Time.time;
        while(Time.time - startTime < secondsBeforeIdle)
            yield return null;

        // Enough time passed, return to idle state
        SwitchState(AIStateMachine.StateName.IDLE);
    }

    private void OnDrawGizmos() 
    {
        if(debug == false || enemy == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(enemy.transform.position + Vector3.up * .5f, .5f);
    }
}
