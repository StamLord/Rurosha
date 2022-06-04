using UnityEngine;

public class SearchAIState : AIState
{
    [SerializeField] private float searchRadius = 5f;
    [SerializeField] private float idleDuration = 5f;
    [SerializeField] private float distanceThreshold = 1f;
    [SerializeField] private float maxSearchTime = 20f;

    private Vector3 lastSeen;
    private Vector3 lastDir;

    private float searchStart;
    
    private Vector3 target;
    private float waitStart;
    private bool waiting;

    protected override void OnEnterState()
    {
        lastSeen = AIStateMachine.enemyLastSeen;
        lastDir = AIStateMachine.enemyLastDir;

        // Subscribe to awareness events
        AIStateMachine.AwarenessAgent.OnSeeAgent += SeeTarget;
        AIStateMachine.AwarenessAgent.OnHearSound += HearSound;

        target = lastSeen;
        MoveTo(lastSeen + lastDir);
        searchStart = Time.time;

        if(debug)
            AIStateMachine.SetDebugColor(Color.yellow);
    }

    public override void OnStateUpdate()
    {
        // End search when time passes
        if(Time.time - searchStart >= maxSearchTime)
        {
            AIStateMachine.SwitchState(0);
            return;
        }

        LookTowards(lastSeen);

        if(waiting == false)
        {
            if(TooFar(target, distanceThreshold) == false)
            {
                waiting = true;
                waitStart = Time.time;
            }
        }
        else if(Time.time - waitStart >= idleDuration)
        {
            target = GenerateNextPosition(lastSeen + lastDir, searchRadius, searchRadius);
            if(MoveTo(target))
                waiting = false;
        }
    }

    protected override void OnExitState()
    {
        AIStateMachine.AwarenessAgent.OnSeeAgent -= SeeTarget;
    }

    private void SeeTarget(StealthAgent agent)
    {
        // Switch to FightAIState
        AIStateMachine.enemy = agent;
        AIStateMachine.SwitchState(1);
    }

    private void HearSound(Vector3 origin)
    {
        target = origin;
        if(MoveTo(target))
            waiting = false;
    }
    
    private void OnDrawGizmos() 
    {
        if(debug == false) return;

        Vector3 size = new Vector3(1f, 2f, 1f);
        Color c = Color.red;
        
        // Last seen position
        Gizmos.color = c;
        Gizmos.DrawCube(lastSeen + Vector3.up * .5f, size);

        // Last direction projection
        c.a = .5f;
        Gizmos.color = c;
        Gizmos.DrawCube(lastSeen + lastDir + Vector3.up * .5f, size);
    }
}
