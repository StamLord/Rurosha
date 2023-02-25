using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLeg : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private float moveRadius = .5f;

    [Header("Animation")]
    [SerializeField] private float animationDuration = .5f;
    [SerializeField] private AnimationCurve animationHeight;

    [Header("Ground")]
    [SerializeField] private float raycastDistance = 1f;
    [SerializeField] private LayerMask groundMask;

    private bool isMoving;

    private void Start() 
    {
        transform.position = target.position;    
    }

    private void Update() 
    {
        // Don't do anything while leg is moving
        if(isMoving) return;

        // Move if too far
        float distance = Vector3.Distance(transform.position, target.position);
        if(distance > moveRadius)
            Move();
    }

    private void Move()
    {
        Vector3 newPos = GetNewPosition();
        StartCoroutine(AnimateMove(newPos));
    }

    private IEnumerator AnimateMove(Vector3 newPosition)
    {
        isMoving = true;

        float startTime = Time.time;
        Vector3 startPos = target.position;

        while(Time.time - startTime <= animationDuration)
        {
            float p = (Time.time - startTime) / animationDuration;

            // Move leg to target
            Vector3 position = Vector3.Lerp(startPos, newPosition, p); 

            // Animate leg raise
            position.y = animationHeight.Evaluate(p);

            target.position = position;

            yield return null;
        }

        // Make sure we are at final position and update lastPosition
        target.position = newPosition;

        isMoving = false;
    }

    private Vector3 GetNewPosition()
    {
        // Find grounded position
        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundMask))
            return hit.point;

        // Default to our position
        return transform.position;
    }

}
