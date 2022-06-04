using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIInput : MonoBehaviour
{
    [SerializeField] private InputState inputState;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private NavMeshPath path = new NavMeshPath();
    [SerializeField] private float pointDistance = .2f;
    [SerializeField] private int pathPoint;
    [SerializeField] private int pathPoints;
    [SerializeField] private bool pathStarted;
    [SerializeField] private Vector3 lastTarget;

    private RaycastHit sweepHit;
    private bool jumpStarted;

    private void Start() 
    {
        navMeshAgent.isStopped = true;
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
        pathPoints = path.corners.Length;
        pathPoint = 0;
        pathStarted = true;
        lastTarget = target;
        return navMeshAgent.CalculatePath(target, path);
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
        if(pathStarted && pathPoint < path.corners.Length)
        {
            // Prepare flat direction
            Vector3 dir = GetPathPosition() - transform.position;
            dir.y = 0;

            // Perform input
            inputState.AxisInput = dir;
            
            bool sweepTest = rigidbody.SweepTest(dir, out sweepHit, 1f);
            if(sweepTest && sweepHit.point.y > transform.position.y)
                if(jumpStarted == false)
                    StartCoroutine("Jump", 1f);


            // Advance to next point if close enough and end path if last point
            if(Vector3.Distance(transform.position, GetPathPosition()) < pointDistance)
                pathStarted = AdvancePathPosition();
        }
        else
            inputState.AxisInput = Vector3.zero;
    }

    private IEnumerator Jump(float holdTime)
    {
        Debug.Log("Jump");
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
        Debug.Log("JumpEnd");
    }

    private void OnDrawGizmos() 
    {
        if(path == null) return;
        
        for (var i = 0; i < path.corners.Length; i++)
        {
            Gizmos.color = (i < pathPoint)? Color.gray : Color.red;
            Gizmos.DrawSphere(path.corners[i], .2f);
            if(i > 0)
                Gizmos.DrawLine(path.corners[i-1], path.corners[i]);
        }
        
    }
}