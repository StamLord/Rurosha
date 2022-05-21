using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    public enum Direction {down, up}
    public enum GroundDetectionType {SphereCast, FourRays}

    [Header("Ground Info")]
    [SerializeField] private float groundSlope;
    [SerializeField] private float maxGroundedSlope = 50;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Vector3 groundNormal;

    public bool IsGrounded {get{return isGrounded;}}
    public float GroundSlope {get{return groundSlope;}}
    public Vector3 GroundNormal {get{return groundNormal;}}

    [Space(20f)]

    [Header("Detection Settings")]
    [SerializeField] private Direction direction;
    [SerializeField] private GroundDetectionType detectionType;
    [SerializeField] private float groundSphereRadius = .3f;
    [SerializeField] private float groundDistance = .8f;
    [SerializeField] private LayerMask groundMask;

    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private Color debugColor = Color.blue;

    void Update()
    {
        GroundCheck();
    }

    private bool GroundCheck()
    {
        Vector3 dir = Vector3.down;

        switch(direction)
        {
            case Direction.up:
                dir = Vector3.up;
                break;
        }

        RaycastHit groundHit;

        switch(detectionType)
        {
            case GroundDetectionType.SphereCast:
                isGrounded = Physics.SphereCast(transform.position, groundSphereRadius, dir, out groundHit, groundDistance, groundMask);
                groundSlope = Vector3.Angle(groundHit.normal, Vector3.up);
                break;
            case GroundDetectionType.FourRays:

                Vector3 averageNormal = Vector3.zero;

                bool ray1 = Physics.Raycast(transform.position + new Vector3(-1,0,1) * groundSphereRadius, dir, out groundHit, groundDistance, groundMask);
                
                if(ray1)
                {
                    averageNormal += groundHit.normal;
                }
                
                bool ray2 = Physics.Raycast(transform.position + new Vector3(1,0,1) * groundSphereRadius, dir, out groundHit, groundDistance, groundMask);
                if(ray2)
                {
                    averageNormal += groundHit.normal;
                }

                bool ray3 = Physics.Raycast(transform.position + new Vector3(-1,0,-1) * groundSphereRadius, dir, out groundHit, groundDistance, groundMask);
                if(ray3)
                {
                    averageNormal += groundHit.normal;
                }

                bool ray4 = Physics.Raycast(transform.position + new Vector3(1,0,-1) * groundSphereRadius, dir, out groundHit, groundDistance, groundMask);
                if(ray4)
                {
                    averageNormal += groundHit.normal;
                }

                averageNormal.Normalize();
                groundNormal = averageNormal;
                groundSlope = Vector3.Angle(averageNormal, Vector3.up);
                isGrounded = ray1 || ray2 || ray3 || ray4;
                isGrounded = isGrounded && groundSlope < maxGroundedSlope;
                break;
        }
        

        return isGrounded;
    }

    private void OnDrawGizmos()
    {
        if(debugView == false) return;

        Vector3 dir = Vector3.down;

        switch(direction)
        {
            case Direction.up:
                dir = Vector3.up;
                break;
        }
        
        Gizmos.color = debugColor;
        switch(detectionType)
        {
            case GroundDetectionType.SphereCast:
                Gizmos.DrawWireSphere(transform.position + dir * groundDistance, groundSphereRadius);
                break;
            case GroundDetectionType.FourRays:
                Gizmos.DrawRay(transform.position + new Vector3(-1,0,1) * groundSphereRadius, dir * groundDistance);
                Gizmos.DrawRay(transform.position + new Vector3(1,0,1) * groundSphereRadius, dir * groundDistance);
                Gizmos.DrawRay(transform.position + new Vector3(-1,0,-1) * groundSphereRadius, dir * groundDistance);
                Gizmos.DrawRay(transform.position + new Vector3(1,0,-1) * groundSphereRadius, dir * groundDistance);
                break;
        }
    }
}
