using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIInput : MonoBehaviour
{
    [SerializeField] private InputState inputState;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private NavMeshPath path; //= new NavMeshPath();
    [SerializeField] private float pointDistance = .2f;
    [SerializeField] private int pathPoint;
    [SerializeField] private int pathPoints;
    [SerializeField] private bool pathStarted;
    [SerializeField] private Vector3 lastTarget;
    [SerializeField] private bool jumpOverObstacles;
    
    [SerializeField] private bool avoidObstacles;
    [SerializeField] private float avoidRadius = 1;
    [SerializeField] private float avoidForce = 1;
    [SerializeField] private LayerMask avoidMask;

    private RaycastHit sweepHit;
    private bool jumpStarted;

    private bool isOverrideMovement;
    private Vector3 overrideVector;

    public Vector3 GetLastPosition()
    {
        return path.corners[path.corners.Length - 1];
    }

    public bool CalculatePath(Vector3 target)
    {
        // Try to sample target on NavMesh
        NavMeshHit hit; 
        bool onNavMesh = NavMesh.SamplePosition(target, out hit, 2f, 1);
        if(onNavMesh)
            target = hit.position;
        else
            return false;
        
        // If target close enough to last target, dont calculate   
        if(Vector3.Distance(lastTarget, target) < pointDistance)
            return false;

        // If close enough to target, dont calculate
        if(Vector3.Distance(transform.position, target) < pointDistance)
            return false;
       
        if(path == null)
            path = new NavMeshPath();

        navMeshAgent.enabled = true;
        bool success = navMeshAgent.CalculatePath(target, path);
        navMeshAgent.enabled = false;

        if(success)
        {
            pathStarted = true;
            pathPoints = path.corners.Length;
            pathPoint = 0;
            lastTarget = target;
        }

        return success;
    }

    public Vector3 GetPathPosition()
    {   
        if(path == null || path.corners.Length == 0)
            return transform.position;
        
        return path.corners[pathPoint];
    }

    public void ClearPath()
    {
        path.ClearCorners();
        pathStarted = false;
    }

    public bool AdvancePathPosition()
    {
        if(pathPoint < path.corners.Length - 1)
        { 
            pathPoint++;
            return true;
        }
        return false;
    }

    private void Update() 
    {
        if(isOverrideMovement)
        {
            inputState.AxisInput = overrideVector;
            return;
        }

        if(pathStarted && pathPoint < path.corners.Length)
        {
            // Advance to next point if close enough and end path if last point
            // Needs to be checked before everything else, since the first point on a path is origin and we are already close enough to it.
            if(Vector3.Distance(transform.position, GetPathPosition()) < pointDistance)
                pathStarted = AdvancePathPosition();

            // Prepare flat direction
            Vector3 dir = GetPathPosition() - transform.position;
            dir.y = 0;

            // NPC avoidance
            if(avoidObstacles)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, avoidRadius, avoidMask);
                Vector3 delta = Vector3.zero;
                foreach(Collider c in colliders)
                {
                    if(c.transform.root == transform.root) continue;
                    delta += (transform.position - c.transform.position) * avoidForce;
                }
                Debug.Log(delta);
                dir += delta;
            }

            // Perform input
            inputState.AxisInput = transform.InverseTransformDirection(dir);
            
            // Jump if encounter obstacle
            if(jumpOverObstacles)
            {
                bool sweepTest = rigidbody.SweepTest(dir, out sweepHit, 1f);
                Debug.Log(sweepHit.transform.name);
                if(sweepTest && sweepHit.point.y > transform.position.y)
                    if(jumpStarted == false)
                        StartCoroutine("Jump", 1f);
            }
        }
        else
            inputState.AxisInput = Vector3.zero;
    }

    private IEnumerator Jump(float holdTime)
    {
        jumpStarted = true;
        float timeStarted = Time.time;

        inputState.Jump.Set(VButtonState.PRESS_START);
        yield return null;
        inputState.Jump.Set(VButtonState.PRESSED);

        while(Time.time - timeStarted < holdTime)
            yield return null;

        inputState.Jump.Set(VButtonState.PRESS_END);
        yield return null;
        inputState.Jump.Set(VButtonState.UNPRESSED);
        jumpStarted = false;
    }

    public void PressButton(string button)
    {
        StartCoroutine("SimulateButtonPress", button);
    }

    public void HoldButton(string button)
    {
        StartCoroutine("SimulateHoldButton", button);
    }

    public void StopHoldButton(string button)
    {
        StartCoroutine("SimulateStopHoldButton", button);
    }

    private VButton GetVButton(string button)
    {
        VButton b = inputState.MouseButton1;

        switch(button)
        {
            case "MB1":
                b = inputState.MouseButton1;
                break;
            case "MB2":
                b = inputState.MouseButton2;
                break;
            case "1":
                b = inputState.Num1;
                break;
            case "2":
                b = inputState.Num2;
                break;
            case "3":
                b = inputState.Num3;
                break;
            case "4":
                b = inputState.Num4;
                break;
            case "5":
                b = inputState.Num5;
                break;
            case "6":
                b = inputState.Num6;
                break;
            case "7":
                b = inputState.Num7;
                break;
            case "8":
                b = inputState.Num8;
                break;
            case "9":
                b = inputState.Num9;
                break;
            case "0":
                b = inputState.Num0;
                break;
            case "defense":
                b = inputState.Defend;
                break;
            case "run":
                b = inputState.Run;
                break;
        }

        return b;
    }

    private IEnumerator SimulateButtonPress(string button)
    {
        VButton b = GetVButton(button);
        b.Set(VButtonState.PRESS_START);
        yield return null;
        b.Set(VButtonState.PRESS_END);
        yield return null;
        b.Set(VButtonState.UNPRESSED);
    }

    private IEnumerator SimulateHoldButton(string button)
    {
        VButton b = GetVButton(button);
        b.Set(VButtonState.PRESS_START);
        yield return null;
        b.Set(VButtonState.PRESSED);
    }

    private IEnumerator SimulateStopHoldButton(string button)
    {
        VButton b = GetVButton(button);
        b.Set(VButtonState.PRESS_END);
        yield return null;
        b.Set(VButtonState.UNPRESSED);
    }

    public void StartOverrideMovement(Vector3 input)
    {
        isOverrideMovement = true;
        overrideVector = input;
    }

    public void StopOverrideMovement()
    {
        isOverrideMovement = false;
        overrideVector = Vector3.zero;
    }

    private void OnDrawGizmos() 
    {
        if(avoidObstacles)
            Gizmos.DrawWireSphere(transform.position, avoidRadius);


        if(path == null) return;
        
        for (var i = 0; i < path.corners.Length; i++)
        {
            Gizmos.color = (i < pathPoint)? Color.gray : Color.yellow;
            Gizmos.DrawSphere(path.corners[i], .2f);
            if(i > 0)
                Gizmos.DrawLine(path.corners[i-1], path.corners[i]);
        }
    }
}
