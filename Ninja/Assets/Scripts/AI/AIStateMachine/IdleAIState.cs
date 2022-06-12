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
    [SerializeField] private float investigateSounds;

    private float waitStart;
    private bool waiting;
    private Vector3 target;

    protected override void OnEnterState()
    {
        target = GenerateNextPosition(transform.position, minRoamRadius, maxRoamRadius);
        AIStateMachine.CharacterStats.OnHit += Hit;
        AIStateMachine.AwarenessAgent.OnSeeAgent += SeeTarget;
        AIStateMachine.AwarenessAgent.OnHearSound += HearSound;
        waiting = true;

        if(debug)
            AIStateMachine.SetDebugColor(Color.white);
    }

    public override void OnStateUpdate()
    {
        // Turn towards next position along path
        LookTowards(AIStateMachine.GetNextPosition());

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
        AIStateMachine.CharacterStats.OnHit -= Hit;
        AIStateMachine.AwarenessAgent.OnSeeAgent -= SeeTarget;
        AIStateMachine.AwarenessAgent.OnHearSound -= HearSound;
    }
    
    private void Hit()
    {
        // Switch to SearchAIState
        AIStateMachine.SwitchState(2);
    }

    private void SeeTarget(StealthAgent agent)
    {
        // Considered enemy?
        string targetFaction = agent.GetComponent<CharacterStats>().Faction;
        float relation = AIStateMachine.CharacterStats.GetRelationship(targetFaction);
        if(relation > -.5f)
            return;

        // Brave enough?
        
        // Switch to FightAIState
        AIStateMachine.enemy = agent;
        AIStateMachine.SwitchState(1);
    }

    private void HearSound(Vector3 origin)
    {
        // Switch to InvestigateAIState
    }

    private void OnDrawGizmos() 
    {
        if(debug == false) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(target + Vector3.up * .5f, .5f);
    }
}
