using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTBrain : MonoBehaviour
{
    [SerializeField] private InputState inputState;
    [SerializeField] private BTSelector entry;

    [SerializeField] private AwarenessAgent awarenessAgent;

    [SerializeField] private float sightMemory = 3f;
    [SerializeField] private float lastSeen;

    [SerializeField] private bool avoidObstacles;
    [SerializeField] private int avoidanceNumberOfRays;
    [SerializeField] private float avoidanceAngle;
    [SerializeField] private float avoidanceRange;
    [SerializeField] private float avoidanceScalar;
    [SerializeField] private LayerMask avoidanceMask;

    [SerializeField] private bool debug;
    [SerializeField] private Color debugRayColor = Color.red;
    [SerializeField] private Color debugFinalColor = Color.blue;

    void Start() 
    {
        List<BTNode> nodes1 = new List<BTNode>()
        {
            new BTSequence(this, new List<BTNode>() 
            {
                new BTActionNode(this, SeeAnyone),
                new BTActionNode(this, Flee),

            }),

            new BTSequence(this, new List<BTNode>() 
            {
                new BTActionNode(this, Idle)
            })

        };
        entry = new BTSelector(this, nodes1);
    }

    void Update()
    {
        entry.Evaluate();
    }

    NodeStates Idle ()
    {
        inputState.AxisInput = Vector3.zero;
        return NodeStates.SUCCESS;
    }

    NodeStates SeeAnyone ()
    {
        if(awarenessAgent.VisibleAgents.Count > 0)
            lastSeen = Time.time;

        if(Time.time - lastSeen < sightMemory)
            return NodeStates.SUCCESS;
        else
            return NodeStates.FAILURE;
    }

    NodeStates Flee ()
    {
        float closestDistance = Mathf.Infinity;
        StealthAgent closest = null;

        inputState.run.Set(VButtonState.PRESSED);
        foreach(StealthAgent a in awarenessAgent.VisibleAgents)
        {
            float distance = Vector3.Distance(transform.position, a.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closest = a;
            }
        }
        
        if(closest != null)
        {
            Vector3 axisInput = -1 * CalculateAxisInput(closest.transform.position);
            inputState.AxisInput = axisInput;

            Quaternion changeInRotation = Quaternion.FromToRotation(Vector3.forward, axisInput);
            Vector3 euler = changeInRotation.eulerAngles;

            inputState.rotation = euler.y;
        }

        return NodeStates.SUCCESS;
    }

    private Vector3 CalculateAxisInput(Vector3 worldSpaceGoal)
    {
        Vector3 direction = worldSpaceGoal - transform.position;
        direction.y = 0;

        if(avoidObstacles) 
            direction += AvoidingAgent.CalculateAvoidanceVector(
                transform, 
                avoidanceAngle, 
                avoidanceNumberOfRays, 
                avoidanceRange,
                avoidanceMask,
                avoidanceScalar);

        direction.Normalize();

        return direction;
    }

    private void OnDrawGizmos() 
    {
        if(!debug) return;

        Gizmos.color = debugRayColor;

        float anglePerLoop = avoidanceAngle / (avoidanceNumberOfRays - 1);
        float halfAngle = avoidanceAngle / 2;
        for (var i = 0; i < avoidanceNumberOfRays; i++)
        {
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0, (anglePerLoop * i ) - halfAngle, 0) * transform.forward * avoidanceRange);
        }

        Gizmos.color = debugFinalColor;
        Gizmos.DrawRay(transform.position, AvoidingAgent.CalculateAvoidanceVector(transform, avoidanceAngle, avoidanceNumberOfRays, avoidanceRange, avoidanceMask, avoidanceScalar));

    }
}
