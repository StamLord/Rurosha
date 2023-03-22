using System.Collections;
using UnityEngine;

public class IKLeg : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private float moveRadius = .5f;
    [SerializeField] private IKLeg[] peers;

    [Header("Animation")]
    [SerializeField] private float animationDuration = .5f;
    [SerializeField] private AnimationCurve animationHeight;

    [Header("Ground")]
    [SerializeField] private float raycastDistance = 1f;
    [SerializeField] private LayerMask groundMask;

    [Header("Default Position")]
    [SerializeField] private Vector3 defaultOffset;

    private bool isMoving;
    public bool IsMoving {get{return isMoving;}}

    private Vector3 groundedPosition;
    private bool foundGround;

    private void Update() 
    {
        // Get ground position underneath us
        groundedPosition = GetGroundedPosition();

        // Don't do anything while leg is moving
        if(isMoving) return;

        // If not grounded just update target
        if(foundGround == false)
        {
            target.position = transform.position + defaultOffset;
        }
        else
        {
            // Move if too far
            float distance = Vector3.Distance(groundedPosition, target.position);
            if(distance > moveRadius)
                Move();
        }
    }

    private bool CanMove()
    {
        for (var i = 0; i < peers.Length; i++)
        {
            if(peers[i].IsMoving)
                return false;
        }

        return true;
    }

    private void Move()
    {
        if(CanMove() == false) return;
            
        StartCoroutine(AnimateMove());
    }

    private IEnumerator AnimateMove()
    {
        isMoving = true;

        float startTime = Time.time;
        Vector3 startPos = target.position;

        while(Time.time - startTime <= animationDuration)
        {
            float p = (Time.time - startTime) / animationDuration;

            // Move leg to target
            Vector3 position = Vector3.Lerp(startPos, groundedPosition, p); 

            // Animate leg raise
            position.y = animationHeight.Evaluate(p);

            target.position = position;

            yield return null;
        }

        // Make sure we are at final position and update lastPosition
        target.position = groundedPosition;

        isMoving = false;
    }

    private Vector3 GetGroundedPosition()
    {
        // Find grounded position
        RaycastHit hit;
        
        foundGround = Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundMask);

        if(foundGround)
            return hit.point;

        // Default to our position
        return transform.position + defaultOffset;
    }

    private void OnDrawGizmos() 
    {
        // Draw current positions
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(target.position, .2f);

        // Draw ground target positions
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(groundedPosition, .2f);
    }

}
