using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    public enum GroundDetectionType {SphereCast, FourRays}

    [Header("Ground Info")]
    [SerializeField] private float groundSlope;
    [SerializeField] private bool isGrounded;
    public bool IsGrounded {get{return isGrounded;}}

    [Space(20f)]

    [Header("Detection Settings")]
    [SerializeField] private GroundDetectionType detectionType;
    [SerializeField] private float groundSphereRadius = .3f;
    [SerializeField] private float groundDistance = .8f;
    [SerializeField] private LayerMask groundMask;

    [Space(20f)]
    
    [Header("Debug View")]
    [SerializeField] private bool debugView;

    void Update()
    {
        GroundCheck();
        DebugView();
    }

    private bool GroundCheck()
    {
        RaycastHit groundHit;

        switch(detectionType)
        {
            case GroundDetectionType.SphereCast:
                isGrounded = Physics.SphereCast(transform.position, groundSphereRadius, Vector3.down, out groundHit, groundDistance, groundMask);
                groundSlope = Vector3.Angle(groundHit.normal, Vector3.up);
                break;
            case GroundDetectionType.FourRays:

                Vector3 averageNormal = Vector3.zero;

                bool ray1 = Physics.Raycast(transform.position + new Vector3(-1,0,1) * groundSphereRadius, Vector3.down, out groundHit, groundDistance, groundMask);
                
                if(ray1)
                {
                    averageNormal += groundHit.normal;
                }
                
                bool ray2 = Physics.Raycast(transform.position + new Vector3(1,0,1) * groundSphereRadius, Vector3.down, out groundHit, groundDistance, groundMask);
                if(ray2)
                {
                    averageNormal += groundHit.normal;
                }

                bool ray3 = Physics.Raycast(transform.position + new Vector3(-1,0,-1) * groundSphereRadius, Vector3.down, out groundHit, groundDistance, groundMask);
                if(ray3)
                {
                    averageNormal += groundHit.normal;
                }

                bool ray4 = Physics.Raycast(transform.position + new Vector3(1,0,-1) * groundSphereRadius, Vector3.down, out groundHit, groundDistance, groundMask);
                if(ray4)
                {
                    averageNormal += groundHit.normal;
                }

                averageNormal.Normalize();

                isGrounded = ray1 || ray2 || ray3 || ray4;
                groundSlope = Vector3.Angle(averageNormal, Vector3.up);
                break;
        }
        

        return isGrounded;
    }

    private void DebugView()
    {
        if(debugView == false) return;

        Debug.DrawRay(transform.position + new Vector3(-1,0,1) * groundSphereRadius, Vector3.down * groundDistance, Color.blue);
        Debug.DrawRay(transform.position + new Vector3(1,0,1) * groundSphereRadius, Vector3.down * groundDistance, Color.blue);
        Debug.DrawRay(transform.position + new Vector3(-1,0,-1) * groundSphereRadius, Vector3.down * groundDistance, Color.blue);
        Debug.DrawRay(transform.position + new Vector3(1,0,-1) * groundSphereRadius, Vector3.down * groundDistance, Color.blue);
    }
}
