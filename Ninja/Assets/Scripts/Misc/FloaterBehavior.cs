using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloaterBehavior : MonoBehaviour
{
    [SerializeField] private InputState inputState;
    [SerializeField] private AwarenessAgent awareness;
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private float maxVelocityChange = 1;

    private enum State {IDLE, ROAM, CHASE, SEARCH}
    [SerializeField] private State state;

    [Header("Idle")]
    [SerializeField] private float idleTime = 5f;
    [SerializeField] private float idleStart = 0;

    [Header("Roam")]
    [SerializeField] private float roamSpeed = 3;
    [SerializeField] private float roamRadius = 5f;
    [SerializeField] private float roamHeightMin = 1f;
    [SerializeField] private float roamHeightMax = 2f;
    private Vector3 roamPos;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 5;
    [SerializeField] private float chaseDistance = 1f;
    [SerializeField] private float chaseHeight = 1.5f;
    [SerializeField] private StealthAgent target;
    private Vector3 lastTargetPos;
    private Vector3 lastTargetVelocity;

    [Header("Search")]
    [SerializeField] private float searchSpeed = 4;
    [SerializeField] private float searchRadius = 3f;
    [SerializeField] private float searchWaitTime = 3;
    [SerializeField] private float maxSearchAttempts = 3;
    [SerializeField] private float turnTime = 1;
    [SerializeField] private float searchWaitStartTime;
    [SerializeField] private bool searchWaitStarted;
    [SerializeField] private int searchAttempt;
    [SerializeField] private Vector3 searchPos;
    [SerializeField] private bool turnStarted;
    [SerializeField] private float turnStartTime;
    [SerializeField] private Vector3 turnDir;

    private void Update()
    {
        if(characterStats.IsAlive == false)
        {
            rigidbody.velocity = Vector3.zero;
            return;
        }
        
        switch(state)
        {
            case State.IDLE:
                Idle();
                break;
            case State.ROAM:
                Roam();
                break;
            case State.CHASE:
                Chase();
                break;
            case State.SEARCH:
                Search();
                break;
        }
        Debug.DrawRay(transform.position, inputState.AxisInput, Color.red);
        ProcessInput();
        
    }

    private void UpdateRotation()
    {
        if(inputState.AxisInput == Vector3.zero) 
            return;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputState.AxisInput, Vector3.up), .2f);
    }

    private void ProcessInput()
    {
        // Input
        Vector3 inputVector = inputState.AxisInput;
        // Don't know why but the AxisInput from BTBrains doesn't need to be transformed
        // Vector3 targetDirection = transform.TransformDirection(inputVector);
        Vector3 targetVelocity = inputVector;

        switch(state)
        {
            case State.ROAM:
                targetVelocity *= roamSpeed;
                break;
            case State.CHASE:
                targetVelocity *= chaseSpeed;
                break;
            case State.SEARCH:
                targetVelocity *= searchSpeed;
                break;
        }

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);;
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Idle()
    {
        inputState.AxisInput = Vector3.zero;

        if((Time.time - idleStart) > idleTime)
        {
            roamPos = GetReachablePointOnCircle(transform.position, roamRadius);
            roamPos.y = Random.Range(roamHeightMin, roamHeightMax);
            state = State.ROAM;
        }

        SeeTest();
    }

    private void Roam()
    {
        Vector3 dir = roamPos - transform.position;
        if(dir.magnitude > 0.5f)
        {
            inputState.AxisInput = dir.normalized;
        }
        else
        {
            idleStart = Time.time;
            state = State.IDLE;
        }

        UpdateRotation();
        SeeTest();
    }

    private bool SeeTest()
    {
        if(awareness.VisibleAgents.Count > 0)
        {
            target = awareness.VisibleAgents[0];
            state = State.CHASE;
            return true;
        }
        return false;
    }

    private void Chase()
    {   
        Vector3 targetPos = target.transform.position;
        if(awareness.VisibleAgents.Contains(target))
        {
            Vector3 fixedTarget = targetPos + Vector3.up * chaseHeight;
            Vector3 dir = fixedTarget - transform.position;

            inputState.AxisInput =(dir.magnitude > chaseDistance)? dir.normalized : Vector3.zero;
            UpdateRotation();

            lastTargetPos = targetPos;
            lastTargetVelocity = target.GetComponent<Rigidbody>().velocity;
        }
        else
        {
            searchPos = lastTargetPos;
            // We take last velocity into account to get general direction
            searchPos += lastTargetVelocity.normalized * 2f;
            awareness.detectRate = 10;
            state = State.SEARCH;
        }
    }

    private void Search()
    {
        // Offset target and get direction
        Vector3 targetPos = searchPos + Vector3.up * chaseHeight;
        Vector3 dir = targetPos - transform.position;
        
        if(searchWaitStarted)
        {
            inputState.AxisInput = Vector3.zero;

            // Enough time spent searching, get next point
            if(Time.time - searchWaitStartTime > searchWaitTime)
            {
                searchWaitStarted = false;
                searchPos = GetReachablePointOnCircle(searchPos, searchRadius);
            }
            else
            {
                // Turn around
                if(turnStarted)
                {
                    float p = (Time.time - turnStartTime) / turnTime;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(turnDir, Vector3.up), p);

                    if(p >= 1)
                        turnStarted = false;
                }
                else
                {
                    turnDir = (Random.Range(0,2) == 0)? -Vector3.right : Vector3.right;
                    turnStarted = true;
                    turnStartTime = Time.time;
                }
            }
        }
        else
        {
            // Reached point, start waiting
            if(dir.magnitude < .5f)
            {
                inputState.AxisInput = Vector3.zero;
                searchWaitStarted = true;
                searchAttempt++;
                searchWaitStartTime = Time.time;
            }
            // Move to position
            else
            {
                inputState.AxisInput = dir.normalized;
                UpdateRotation();
            }
        }
        
        // If target in sight and we switch to CHASE, reset attempts
        if(SeeTest())
        {
            searchAttempt = 0;
            searchWaitStarted = false;
        }
        // Change to IDLE if max attempts reached
        else if (searchAttempt > maxSearchAttempts)
        {
            awareness.detectRate = 1;
            state = State.IDLE;
            searchAttempt = 0;
            searchWaitStarted = false;
        }
    }

    private Vector3 GetReachablePointOnSphere(Vector3 center, float radius)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * radius;
        return GetPointOnObstacle(center, randomPoint);
    }

    private Vector3 GetReachablePointOnCircle(Vector3 center, float radius)
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        Debug.Log(randomPoint);
        Vector3 point = center + new Vector3(randomPoint.x, 0, randomPoint.y);
        Debug.Log(point);
        return GetPointOnObstacle(center, point);
    }

    private Vector3 GetPointOnObstacle(Vector3 origin, Vector3 target)
    {
        Vector3 dir = target - origin;
        RaycastHit hit;
        if(Physics.Raycast(origin, dir, out hit, dir.magnitude))
            return hit.point;
        else 
            return target;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;

        switch(state)
        {
            case State.ROAM:
                Gizmos.DrawSphere(roamPos, .5f);
                break;
            case State.CHASE:
                Gizmos.DrawSphere(target.transform.position, .5f);
                break;
            case State.SEARCH:
                Gizmos.DrawSphere(searchPos, .5f);
                break;
        }
    }
}