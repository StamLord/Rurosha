using UnityEngine;
using System.Collections;

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

    private float lookStartTime;
    private float lookDuration = 1f;
    private bool looking;
    private Coroutine lookThenMove;

    protected override void OnEnterState()
    {
        lastSeen = AIStateMachine.enemyLastSeen;
        lastDir = AIStateMachine.enemyLastDir;

        // Subscribe to awareness events
        AIStateMachine.AwarenessAgent.OnSeeAgent += SeeTarget;
        AIStateMachine.AwarenessAgent.OnHearSound += HearSound;

        // Alertness
        AIStateMachine.AwarenessAgent.SetAlert(true);

        target = lastSeen + lastDir;
        MoveTo(target);
        searchStart = Time.time;

        if(debug)
            AIStateMachine.SetDebugColor(Color.yellow);
    }

    public override void OnStateUpdate()
    {
        // End search when time passes
        if(Time.time - searchStart >= maxSearchTime)
        {
            // Switch to IdleAIState
            AIStateMachine.SwitchState(0);
            return;
        }

        // Look towrads target location unless running LookThenMove coroutine
        if(looking == false)
            LookTowards(target);
        
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
        AIStateMachine.AwarenessAgent.OnHearSound -= HearSound;
        AIStateMachine.AwarenessAgent.SetAlert(false);
    }

    private void SeeTarget(StealthAgent agent)
    {
        // Switch to FightAIState
        AIStateMachine.enemy = agent;
        AIStateMachine.SwitchState(1);
    }

    private void HearSound(Vector3 origin)
    {
        if(looking)
            StopCoroutine(lookThenMove);
        else
            lookStartTime = Time.time; // Only get time if on first sound so multiple sound don't reset count
        
        lookThenMove = StartCoroutine("LookThenMove", origin);
    }

    private IEnumerator LookThenMove(Vector3 target)
    {
        looking = true;

        while(Time.time - lookStartTime < lookDuration)
        {
            LookTowards(target);
            yield return null;
        }

        this.target = target;
        if(MoveTo(target))
            waiting = false;

        looking = false;
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
