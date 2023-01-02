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
    
    [Tooltip("Fight enemy on sight")]
    [SerializeField] private bool aggressive;
    
    [Tooltip("Fight back if attacked")]
    [SerializeField] private bool retaliate;
    
    [Tooltip("Run away from enemies")]
    [SerializeField] private bool cowardly;

    [SerializeField] private ScheduleAgent scheduleAgent;
    [SerializeField] private TownManager townManager;

    [SerializeField]private float waitStart;
    [SerializeField]private bool waiting;
    [SerializeField]private Vector3 target;

    protected override void OnEnterState()
    {
        target = GenerateNextPosition(transform.position, minRoamRadius, maxRoamRadius);
        AIStateMachine.CharacterStats.OnHitBy += Hit;
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
            // Check distance to last position of navmesh path
            if(TooFar(GetLastPosition(), distanceThreshold) == false)
            {
                waiting = true;
                waitStart = Time.time;
            }
        }
        else if(Time.time - waitStart >= idleDuration)
        {
            if(townManager && scheduleAgent)
            {
                Task t = scheduleAgent.GetCurrentTask();
                Vector3 coords;
                float radius;
                bool locationExists = townManager.GetLocation(t.location, out coords, out radius);

                if(locationExists)
                    target = GenerateNextPosition(coords, radius, radius);
                else
                    target = GenerateNextPosition(transform.position, minRoamRadius, maxRoamRadius);
            }
            else
                target = GenerateNextPosition(transform.position, minRoamRadius, maxRoamRadius);

            if(MoveTo(target))
                waiting = false;
        }
    }

    protected override void OnExitState()
    {
        AIStateMachine.CharacterStats.OnHitBy -= Hit;
        AIStateMachine.AwarenessAgent.OnSeeAgent -= SeeTarget;
        AIStateMachine.AwarenessAgent.OnHearSound -= HearSound;
    }
    
    private void Hit(StealthAgent agent)
    {
        // If an ally we don't care
        if(IsAlly(agent))
            return;
        
        AIStateMachine.enemy = agent;

        // If we don't retailiate we flee
        if(retaliate == false)
        {
            AIStateMachine.SwitchState(3);
            return;
        }

        // If enemy is visible we switch to Fight
        foreach(StealthAgent a in AIStateMachine.AwarenessAgent.VisibleAgents)
        {
            if(agent == a)
            {
                AIStateMachine.SwitchState(1);
                return;
            }
        }

        // Otherwise, switch to SearchAIState
        AIStateMachine.SwitchState(2);
    }

    private void SeeTarget(StealthAgent agent)
    {
        // If not an enemy, do nothing
        if(IsEnemy(agent) == false)
            return;
        
        // If cowardly, run way from enemy
        if(cowardly)
        {
            AIStateMachine.enemy = agent;
            AIStateMachine.SwitchState(3);
        }
        // If agrressive, fight
        else if(aggressive)
        {
            AIStateMachine.enemy = agent;
            AIStateMachine.SwitchState(1);
        }
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
