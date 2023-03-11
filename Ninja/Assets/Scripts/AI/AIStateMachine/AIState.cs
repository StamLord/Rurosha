using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : State
{
    [SerializeField] private AIStateMachine aiStateMachine;
    public AIStateMachine AIStateMachine { 
        get { 
            return (aiStateMachine == null)? aiStateMachine = (AIStateMachine)_stateMachine : aiStateMachine; 
            }}

    public CharacterStats Stats { get { return AIStateMachine.CharacterStats;}}
    public StealthAgent StealthAgent { get { return AIStateMachine.StealthAgent;}}

    [SerializeField] protected float turnSpeed = 1f;
    [SerializeField] protected bool debug;

    public void SwitchState(AIStateMachine.StateName state)
    {
        AIStateMachine.SwitchState(state);
    }

    /// <summary>
    /// Finds a path on NavMesh to target
    /// </summary>
    /// <param name="target">Position in world space to find a path to.</param>
    /// <returns>Returns true if found path</returns>
    protected bool MoveTo(Vector3 target)
    {
        return aiStateMachine.CalculatePath(target);
    }

    protected void MoveStop()
    {
        aiStateMachine.ClearPath();
    }

    protected Vector3 GetLastPosition()
    {
        return aiStateMachine.GetLastPosition();
    }
    // TODO: Move look rotation handling into AIInput.cs for consistency
    protected void LookTowards(Vector3 target, float speedMult = 1f)
    {
        Vector3 direction = target - transform.position;

        direction.y = 0;
        direction.Normalize();

        if(direction.magnitude == 0) return;
        transform.forward = Vector3.Lerp(transform.forward, direction, turnSpeed * speedMult * Time.deltaTime);;
    }

    protected bool TooFar(Vector3 target, float distanceThreshold)
    {
        return Vector3.Distance(transform.position, target) > distanceThreshold;
    }

    protected void PressButton(string button)
    {
        AIStateMachine.PressButton(button);
    }

    protected void HoldButton(string button)
    {
        AIStateMachine.HoldButton(button);
    }

    protected void StopHoldButton(string button)
    {
        AIStateMachine.StopHoldButton(button);
    }

    protected void StartOverrideMovement(Vector3 input)
    {
        AIStateMachine.StartOverrideMovement(input);
    }

    protected void StopOverrideMovement()
    {
        AIStateMachine.StopOverrideMovement();
    }

    protected Vector3 GenerateNextPosition(Vector3 origin, float minRoamRadius, float maxRoamRadius)
    {
        if(minRoamRadius == 0 && maxRoamRadius == 0)
            return origin;
        
        float distance = Random.Range(minRoamRadius, maxRoamRadius);
        Vector2 onCircle = Random.insideUnitCircle * distance;
        return new Vector3(onCircle.x, 0, onCircle.y) + origin;
    }

    private float GetRelationship(StealthAgent agent)
    {
        if(agent == null) return 0;
        
        string targetFaction = agent.transform.root.GetComponent<CharacterStats>().Faction;
        return AIStateMachine.CharacterStats.GetRelationship(targetFaction);
    }

    protected bool IsEnemy(StealthAgent agent)
    {
        return GetRelationship(agent) <= -.5f; // -50% and below is an enemy
    }

    protected bool IsAlly(StealthAgent agent)
    {
        return GetRelationship(agent) >= .5f; // 50% and up is an ally
    }
}
