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

    [SerializeField] private MoveBehavior idleBehavior;
    [SerializeField] private MoveBehavior fleeBehavior;

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

    private List<Transform> GetNearbyFlockMembers()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        List<Transform> neighbors = new List<Transform>();

        foreach(Collider c in colliders)
        {
            BTBrain btb = c.GetComponent<BTBrain>();
            if(btb)
                neighbors.Add(btb.transform);
        }
        
        return neighbors;
    }

    NodeStates Idle ()
    {
        inputState.AxisInput = idleBehavior.CalculateMove(transform, flock: GetNearbyFlockMembers());
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
            Vector3 axisInput = fleeBehavior.CalculateMove(transform, closest.transform);
            inputState.AxisInput = axisInput;

            Quaternion changeInRotation = Quaternion.FromToRotation(Vector3.forward, axisInput);
            Vector3 euler = changeInRotation.eulerAngles;

            inputState.rotation = euler.y;
        }

        return NodeStates.SUCCESS;
    }
}
