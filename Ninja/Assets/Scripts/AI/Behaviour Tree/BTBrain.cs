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
    [SerializeField] private MoveBehavior roamBehavior;

    [SerializeField] private bool debug;
    [SerializeField] private Color debugRayColor = Color.red;
    [SerializeField] private Color debugFinalColor = Color.blue;

    [SerializeField] private Transform goal;
    [SerializeField] private bool flat = true;
    [SerializeField] private float timeToWait = 3;

    [SerializeField] private string currentBTNode;

    private Vector3 origin;

    private Dictionary<string, object> blackboard = new Dictionary<string, object>();
    private List<Transform> cachedNeighbors = new List<Transform>();

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
                new BTActionNode(this, Roam),
                new BTSelector(this, new List<BTNode>()
                {
                    new BTActionNode(this, IsCloseEnough),
                    new BTActionNode(this, IsStuck)
                }),
                new BTSelector(this, new List<BTNode>()
                {
                    new BTActionNode(this, Wait),
                    new BTActionNode(this, Idle)
                }),
                new BTActionNode(this, NextRoam)
            })

        };
        entry = new BTSelector(this, nodes1);

        origin = transform.position;
        CalculateNextRoam();        
        
        // Initialize Blackboard
        blackboard["Wait Start Time"] = 0f;
        blackboard["Wait Started"] = false;
        blackboard["Last Position"] = transform.position;
        blackboard["Last Position Check"] = 0f;
    }

    void CalculateNextRoam()
    {
        if(goal == null)
            goal = new GameObject().transform;

        goal.position = origin + (Vector3)Random.insideUnitSphere * 5;
    }

    void Update()
    {
        entry.Evaluate();
        cachedNeighbors = GetNearbyFlockMembers();
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
    
    NodeStates IsCloseEnough ()
    {
        currentBTNode = "IsCloseEnough";
        Vector3 flatGoal = goal.position;
        if(flat) flatGoal.y = 0;

        Vector3 flatLocal = transform.position;
        if(flat) flatLocal.y = 0;

        if(Vector3.Distance(flatLocal, flatGoal) < 1)
        {
            bool waitStarted = (bool)blackboard["Wait Started"];
            
            if(waitStarted == false)
            {
                blackboard["Wait Start Time"] = Time.time;
                blackboard["Wait Started"] = true;
            }

            return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }

    NodeStates IsStuck()
    {
        if(Time.time - (float)blackboard["Last Position Check"] > 2f)
        {
            Vector3 lastPosition = (Vector3)blackboard["Last Position"];
            if(Vector3.Distance(transform.position, lastPosition) < .1f)
            {
                Debug.Log("Stuck!!!!!");
                return NodeStates.SUCCESS;    
            }
            
            // Set new values
            blackboard["Last Position Check"] = Time.time;
            blackboard["Last Position"] = transform.position;
        }

        return NodeStates.FAILURE;
    }

    NodeStates Wait()
    {
        currentBTNode = "Wait";
        float startWait = (float)blackboard["Wait Start Time"];

        if(Time.time - startWait > timeToWait)
        {
            blackboard["Wait Started"] = false;
            return NodeStates.SUCCESS;
        }
        
        return NodeStates.FAILURE;
    }

    NodeStates Idle()
    {
        currentBTNode = "Idle";
        inputState.AxisInput = idleBehavior.CalculateMove(transform, goal, cachedNeighbors);
        return NodeStates.FAILURE;
    }

    NodeStates NextRoam()
    {   
        currentBTNode = "NextRoam";
        CalculateNextRoam();

        return NodeStates.SUCCESS;
    }

    NodeStates Roam()
    {
        currentBTNode = "Roam";

        if(roamBehavior)
        {   
            inputState.AxisInput = roamBehavior.CalculateMove(transform, goal, cachedNeighbors);
        }

        return NodeStates.SUCCESS;
    }

    NodeStates SeeAnyone ()
    {
        currentBTNode = "SeeAnyone";
        if(awarenessAgent.VisibleAgents.Count > 0)
            lastSeen = Time.time;

        if(Time.time - lastSeen < sightMemory)
            return NodeStates.SUCCESS;
        else
            return NodeStates.FAILURE;
    }

    NodeStates Flee ()
    {
        currentBTNode = "Flee";

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
            Vector3 axisInput = fleeBehavior.CalculateMove(transform, closest.transform, cachedNeighbors);
            inputState.AxisInput = axisInput;

            Quaternion changeInRotation = Quaternion.FromToRotation(Vector3.forward, axisInput);
            Vector3 euler = changeInRotation.eulerAngles;

            inputState.rotation = euler.y;
        }

        return NodeStates.SUCCESS;
    }
}
