using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSensor : MonoBehaviour
{
    [Header("Wall Info")]
    [SerializeField] bool wallDetected;
    [SerializeField] Vector3 wallPoint;
    [SerializeField] Vector3 wallNormal;

    public bool WallDetected {get{return wallDetected;}}
    public Vector3 WallPoint {get{return wallPoint;}}
    public Vector3 WallNormal {get{return wallNormal;}}

    [Space(20f)]

    [Header("Detection Settings")]
    [SerializeField] private InputState input;
    [SerializeField] private float wallDistance = 1f;
    [SerializeField] private LayerMask wallLayerMask;

    [Space(20f)]

    [Header("Debug View")]
    [SerializeField] private bool debugView;
    [SerializeField] private Color wallColor = Color.red;

    // Update is called once per frame
    void Update()
    {
        DetectWall();
    }

    private void DetectWall()
    {
        Vector3 direction = transform.TransformDirection(input.AxisInput);
        if (direction == Vector3.zero)
        {
            wallDetected = false;
            return;
        }

        RaycastHit wall;
        Ray wallRay = new Ray(transform.position, direction);
        
        wallDetected = Physics.Raycast(wallRay, out wall, wallDistance, wallLayerMask);

        if (wallDetected)
        {
            wallPoint = wall.point;
            wallNormal = wall.normal;
        }
    }

    private void OnDrawGizmos() 
    {
        if(debugView == false || wallDetected == false) return;

        Gizmos.color = wallColor;
        Gizmos.DrawCube(wallPoint, new Vector3(.1f, .1f, .1f));
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(input.AxisInput) * wallDistance);
    }
}
