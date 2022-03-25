using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSensor : MonoBehaviour
{
    [Header("Wall Info")]
    [SerializeField] bool wallDetected;
    [SerializeField] Vector3 wallPoint;
    [SerializeField] Vector3 wallNormal;
    [SerializeField] float wallAngle;

    public bool WallDetected {get {return wallDetected;}}
    public Vector3 WallPoint {get {return wallPoint;}}
    public Vector3 WallNormal {get {return wallNormal;}}
    public float WallAngle {get {return wallAngle;}}

    [Space(20f)]

    [Header("Detection Settings")]
    [SerializeField] private InputState input;
    [SerializeField] private float wallDistance = 1f;
    [SerializeField] private LayerMask wallLayerMask;

    [Space(20f)]

    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private Color wallColor = Color.red;

    void Update()
    {
        DetectWall();
    }

    private void DetectWall()
    {
        Vector3 direction = transform.TransformDirection(input.AxisInput);
        wallDetected = DetectWall(direction, out wallPoint, out wallNormal, out wallAngle);
    }

    public bool DetectWall(Vector3 direction, out Vector3 point, out Vector3 normal, out float angle)
    {
        if (direction == Vector3.zero)
        {
            point = normal = Vector3.zero;
            angle = 0f;
            return false;
        }

        RaycastHit wall;
        Ray wallRay = new Ray(transform.position, direction);
        
        wallDetected = Physics.Raycast(wallRay, out wall, wallDistance, wallLayerMask);

        if (wallDetected)
        {
            point = wall.point;
            normal = wall.normal;
            angle = Vector3.Angle(normal, transform.forward);
            if(angle > 180f) wallAngle -= 360f;

            return true;
        }

        point = normal = Vector3.zero;
        angle = 0f;
        return false;
    }

    private void OnDrawGizmos() 
    {
        if(debugView == false || wallDetected == false) return;

        Gizmos.color = wallColor;
        Gizmos.DrawCube(wallPoint, new Vector3(.1f, .1f, .1f));
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(input.AxisInput) * wallDistance);
    }
}
