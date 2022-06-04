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

    [SerializeField] protected float turnSpeed = 1f;
    [SerializeField] protected bool debug;

    protected bool MoveTo(Vector3 target)
    {
        return aiStateMachine.CalculatePath(target);
    }

    protected void MoveStop()
    {
        aiStateMachine.ClearPath();
    }

    protected void LookTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;
        direction.Normalize();

        if(direction.magnitude == 0) return;
        transform.forward = Vector3.Lerp(transform.forward, direction, turnSpeed * Time.deltaTime);;
    }

    protected bool TooFar(Vector3 target, float distanceThreshold)
    {
        return Vector3.Distance(transform.position, target) > distanceThreshold;
    }

    protected Vector3 GenerateNextPosition(Vector3 origin, float minRoamRadius, float maxRoamRadius)
    {
        float distance = Random.Range(minRoamRadius, maxRoamRadius);
        Vector2 onCircle = Random.insideUnitCircle * distance;
        return new Vector3(onCircle.x, 0, onCircle.y) + transform.position;
    }
}
