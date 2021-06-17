using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidingAgent : MonoBehaviour
{
    public Transform goal;

    public Rigidbody rigidbody;
    public float velocity = 1;
    public float maxVelocityChange = .5f;
    public bool rotateTowards;

    public float goalScalar;

    public float targetVelocity = 1f;
    public int numberOfRays = 17;
    public float angle = 90;

    public float rayRange = 2f;
    public LayerMask mask;

    public bool debug;

    public Color debugRayColor = Color.red;
    public Color debugFinalColor = Color.blue;

    Vector3 deltaPosition;

    void Update()
    {   
        deltaPosition = CalculateAvoidanceVector(transform, angle, numberOfRays, rayRange, mask, targetVelocity);

        // Add Goal Importance
        if(goal)
            deltaPosition += (goal.position - transform.position).normalized * goalScalar;

        if(rotateTowards) transform.forward = Vector3.Lerp(transform.forward, deltaPosition, .01f);
    }

    public static Vector3 CalculateAvoidanceVector(Transform transform, float angle, int numberOfRays, float rayRange, LayerMask mask, float targetVelocity)
    {
        Vector3 deltaPosition = Vector3.zero;

        float anglePerLoop = angle / (numberOfRays - 1);
        float halfAngle = angle / 2;

        for (var i = 0; i < numberOfRays; i++)
        {
            RaycastHit hitInfo;
            float yAngle = (anglePerLoop * i ) - halfAngle;
            Vector3 direction = Quaternion.Euler(0, yAngle, 0) * transform.forward;

            Ray ray = new Ray(transform.position, direction);

            if(Physics.Raycast(ray, out hitInfo, rayRange, mask))
            {
               // Debug.Log(hitInfo.transform.name);
                deltaPosition -= (1f / numberOfRays) * targetVelocity * direction;
            }
            else
                deltaPosition += (1f / numberOfRays) * targetVelocity * direction;
        }

        return deltaPosition;
    }

    private void FixedUpdate() 
    {
        Vector3 currentVelocity = rigidbody.velocity;
        Vector3 velocityChange = deltaPosition.normalized * velocity - rigidbody.velocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void OnDrawGizmos() 
    {
        if(!debug) return;

        Gizmos.color = debugRayColor;

        float anglePerLoop = angle / (numberOfRays - 1);
        float halfAngle = angle / 2;
        for (var i = 0; i < numberOfRays; i++)
        {
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0, (anglePerLoop * i ) - halfAngle, 0) * transform.forward * rayRange);
        }

        Gizmos.color = debugFinalColor;
        Gizmos.DrawRay(transform.position, deltaPosition);

    }
}
