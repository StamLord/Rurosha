using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTBrain : MonoBehaviour
{
    [SerializeField] private CharacterStats charStats;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private InputState inputState;
    [SerializeField] private BTSelector entry;

    [Header ("Senses")]
    [SerializeField] private AwarenessAgent awarenessAgent;

    [Header ("Behaviors")]
    [SerializeField] private MoveBehavior idleBehavior;
    [SerializeField] private MoveBehavior chaseBehavior;
    [SerializeField] private MoveBehavior fleeBehavior;
    [SerializeField] private MoveBehavior roamBehavior;

    [Header("Chasing Parameters")]
    [SerializeField] private bool isChasing;
    [SerializeField] private Transform chaseTarget;
    [SerializeField] private bool inSight;
    [SerializeField] private float sightMemory = 3f;
    [SerializeField] private float sightCheatRadius = 2f;
    [SerializeField] private float lastSeenTime;
    [SerializeField] private Vector3 lastSeenPos;

    [Header("Attack Parameters")]
    [SerializeField] private float attackRange = 2f;

    [Header("Pathfinding")]
    [SerializeField] private NavMeshPath path;
    [SerializeField] private Vector3[] points = new Vector3[0];
    [SerializeField] private int nextPoint;

    [Header("Debug")]
    [SerializeField] private bool debug;
    [SerializeField] private Color debugRayColor = Color.red;
    [SerializeField] private Color debugFinalColor = Color.blue;

    [SerializeField] private Transform goal;
    [SerializeField] private bool flat = true;
    [SerializeField] private float timeToWait = 3;
    [SerializeField] private float brave = 1;

    [SerializeField] private string currentBTNode;

    private Vector3 origin;

    private Dictionary<string, object> blackboard = new Dictionary<string, object>();
    private List<Transform> cachedNeighbors = new List<Transform>();

    void Start() 
    {
        lastSeenTime = int.MinValue;
        path = new NavMeshPath();

        List<BTNode> nodes1 = new List<BTNode>()
        {
            new BTSelector(this, new List<BTNode>()
            {
                new BTActionNode(this, IsDead),
                // Fight or Flight branch
                new BTSequence(this, new List<BTNode>() 
                {
                    new BTSelector(this, new List<BTNode>()
                    {
                        new BTActionNode(this, ChasingAnyone),
                        new BTActionNode(this, SeeAnyone)
                    }),
                    new BTSelector(this, new List<BTNode>()
                    {
                        new BTSequence(this, new List<BTNode>()
                        {
                            new BTActionNode(this, IsBraveEnough),
                            new BTSelector(this, new List<BTNode>()
                            {
                                new BTSequence(this, new List<BTNode>()
                                {
                                    new BTActionNode(this, IsCloseEnoughToAttack),
                                    new BTSequence(this, new List<BTNode>()
                                    {
                                        new BTActionNode(this, DrawWeapon),
                                        new BTActionNode(this, PrepareAttack),
                                        new BTActionNode(this, Attack)
                                    }),
                                }),
                                new BTSelector(this, new List<BTNode>()
                                {
                                    new BTActionNode(this, IsCloseEnoughToAttack),
                                    new BTActionNode(this, Chase),
                                }),
                            })
                        }),
                        new BTActionNode(this, Flee)
                    })
                }),
                /*
                // Idle and Roam branch
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
                })*/
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
        // Reset Inputs
        inputState.AxisInput = Vector3.zero;
        inputState.Run.Set(VButtonState.UNPRESSED);
        inputState.Jump.Set(VButtonState.UNPRESSED);
        inputState.Crouch.Set(VButtonState.UNPRESSED);
        inputState.MouseButton1.Set(VButtonState.UNPRESSED);
        inputState.MouseButton2.Set(VButtonState.UNPRESSED);

        // AI
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

    NodeStates IsDead()
    {
        if(charStats.IsAlive) 
            return NodeStates.FAILURE;

        return NodeStates.SUCCESS;
    }
    
    NodeStates IsCloseEnoughToGoal()
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

    NodeStates IsBraveEnough()
    {
        currentBTNode = "IsBraveEnough";
        // Debug.Log(currentBTNode);
        return (brave > .7f) ? NodeStates.SUCCESS : NodeStates.FAILURE;
    }

    NodeStates IsCloseEnoughToAttack()
    {
        currentBTNode = "IsCloseEnoughToAttack";
        // Debug.Log(currentBTNode);
        if(chaseTarget)
        {
            if(Vector3.Distance(chaseTarget.position, transform.position) < attackRange)
                return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }

    NodeStates DrawWeapon()
    {
        currentBTNode = "DrawWeapon";
        // Debug.Log(currentBTNode);
        // Draw weapon with highest damage
        weaponManager.SelectBestWeapon();
        return NodeStates.SUCCESS;
    }

    NodeStates PrepareAttack()
    {
        currentBTNode = "PrepareAttack";
        if(blackboard.ContainsKey("Last PrepareAttack") && Time.time < (float)blackboard["Last PrepareAttack"] + 2f)
            return NodeStates.FAILURE;

        blackboard["Last PrepareAttack"] = Time.time;

        // Debug.Log(currentBTNode);
        Item item = weaponManager.GetSelectedItem();
        if(item.GetType() == typeof(Weapon))
        {
            Weapon w = (Weapon)item;
            if(w.WeaponType == WeaponType.SWORD)
            {
                KatanaDirectional kd = weaponManager.GetActiveGameObject().GetComponent<KatanaDirectional>();
                // Pick random direction
                if(kd)
                    kd.SetDirection((Direction9)Random.Range(0,9)); // Random 1..8
            }
        }

        if(blackboard.ContainsKey("Last PrepareAttack Direction") && Time.time < (float)blackboard["Last PrepareAttack Direction"] + 1f)
            return NodeStates.FAILURE;
        blackboard["Last PrepareAttack Direction"] = Time.time;

        return NodeStates.SUCCESS;
    }
    
    NodeStates Attack()
    {
        currentBTNode = "Attack";
        // Debug.Log(currentBTNode);
        inputState.MouseButton1.Set(VButtonState.PRESS_START);
        return NodeStates.SUCCESS;
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
        if((bool)blackboard["Wait Started"] == false)
        {
            blackboard["Wait Start Time"] = Time.time;
            blackboard["Wait Started"] = true;
        }

        if(Time.time - (float)blackboard["Wait Start Time"] > timeToWait)
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
        // Debug.Log(currentBTNode);
        if(awarenessAgent.VisibleAgents.Count > 0)
            lastSeenTime = Time.time;

        if(Time.time - lastSeenTime < sightMemory)
            return NodeStates.SUCCESS;
        else
            return NodeStates.FAILURE;
    }

    NodeStates ChasingAnyone ()
    {
        currentBTNode = "ChasingAnyone";
        // Debug.Log(currentBTNode);
        if(isChasing)
            return NodeStates.SUCCESS;
        else
            return NodeStates.FAILURE;
    }

    NodeStates Chase()
    {
        currentBTNode = "Chase";
        // Debug.Log(currentBTNode);
        // Pick chase target
        if(isChasing == false)
        {
            StealthAgent sa = FindClosestStealthAgent();
            if(sa)
            {
                chaseTarget = sa.transform;
                isChasing = true;
            }
        }

        if(chaseTarget)
        {
            // Check if still seeing target
            if(Vector3.Distance(transform.position, chaseTarget.position) < sightCheatRadius)
            {
                inSight = true;
            }
            else
            {
                inSight = false;
                foreach(StealthAgent sa in awarenessAgent.VisibleAgents)
                {
                    if(sa.transform == chaseTarget)
                    {
                        inSight = true;
                        break;
                    }
                }
            }

            // Update last seen position
            if(inSight)
                lastSeenPos = chaseTarget.position;

            // Generate path if no path or target moved from path's final destination
            if(points.Length == 0 || Vector3.Distance(lastSeenPos, points[points.Length - 1]) > .1f)
            {
                NavMesh.CalculatePath(transform.position, lastSeenPos, NavMesh.AllAreas, path);
                points = path.corners;
                nextPoint = 0;
            }
        }
        
        for (int i = 0; i < points.Length - 1; i++)
        {   
            // Draw path for debugging
            Debug.DrawLine(points[i], points[i + 1], Color.red);
            
            // Look for that farthest point in the path we are close enough to set next point
            if(Vector3.Distance(transform.position, points[i]) < .2f)
                nextPoint = i + 1;
        }

        // Move goal transform at next point
        goal.position = (points.Length > 0) ? points[nextPoint] : transform.position;
        inputState.Run.Set((charStats.Stamina > 20) ? VButtonState.PRESSED : VButtonState.UNPRESSED);
        inputState.AxisInput = chaseBehavior.CalculateMoveFlat(transform, goal, cachedNeighbors);

        return NodeStates.SUCCESS;
    }

    NodeStates Flee ()
    {
        currentBTNode = "Flee";
        StealthAgent closest = FindClosestStealthAgent();
        
        if(closest != null)
        {
            Vector3 axisInput = fleeBehavior.CalculateMove(transform, closest.transform, cachedNeighbors);
            inputState.AxisInput = axisInput;

            // Quaternion changeInRotation = Quaternion.FromToRotation(Vector3.forward, axisInput);
            // Vector3 euler = changeInRotation.eulerAngles;

            // inputState.rotation = euler.y;
        }

        return NodeStates.SUCCESS;
    }

    private StealthAgent FindClosestStealthAgent()
    {
        float closestDistance = Mathf.Infinity;
        StealthAgent closest = null;

        inputState.Run.Set(VButtonState.PRESSED);
        foreach(StealthAgent a in awarenessAgent.VisibleAgents)
        {
            float distance = Vector3.Distance(transform.position, a.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closest = a;
            }
        }

        return closest;
    }

    private void OnDrawGizmosSelected() 
    {
        if(debug == false) return;

        Color color1 = Color.yellow;
        Color color2 = Color.red;

        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.color = Color.Lerp(color1, color2, (float)i/points.Length);
            Gizmos.DrawSphere(points[i], .2f);
        }

        Gizmos.color = color1;
        Gizmos.DrawWireSphere(transform.position, sightCheatRadius);
    }
}
