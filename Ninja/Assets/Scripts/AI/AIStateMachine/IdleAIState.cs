using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleAIState : AIState
{
    [SerializeField] private float minRoamRadius = 5f;
    [SerializeField] private float maxRoamRadius = 10f;
    [SerializeField] private float idleDuration = 5f;
    [SerializeField] private float distanceThreshold = 1f;

    private float waitStart;
    private bool waiting;
    private Vector3 target;

    protected override void OnEnterState()
    {
        target = GenerateNextPosition(transform.position, minRoamRadius, maxRoamRadius);
        AIStateMachine.AwarenessAgent.OnSeeAgent += SeeTarget;
        waiting = true;

        if(debug)
            AIStateMachine.SetDebugColor(Color.white);
    }

    public override void OnStateUpdate()
    {
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
            target = GenerateNextPosition(transform.position, minRoamRadius, maxRoamRadius);
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
        // Considered enemy?
        // Brave enough?
        // Switch to FightAIState
        AIStateMachine.enemy = agent;
        AIStateMachine.SwitchState(1);
    }

    private void OnDrawGizmos() 
    {
        if(debug == false) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(target + Vector3.up * .5f, .5f);
    }
}
