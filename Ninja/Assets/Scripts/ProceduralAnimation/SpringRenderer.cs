using UnityEngine;
using System;

public class SpringRenderer : MonoBehaviour
{
    [Header("Spring")]
    [SerializeField] private SpringJoint springJoint;
    
    [Header("Draw Line")]
    [SerializeField] private bool drawLine;
    [SerializeField] private LineRenderer lineRenderer;
    
    [Header("Dynamic Width")]
    [SerializeField] private bool dynamicWidth;
    [SerializeField] private float defaultWidth = .1f;
    [SerializeField] private Vector2 lineWidthMinMax = new Vector2(0.01f, 0.1f);
    [SerializeField] private float maxStretchForce = 1000;

    [Header("Collisions")]
    [SerializeField] private bool enableCollisions;
    [SerializeField] private new CapsuleCollider collider;

    [Header("IK")]
    [SerializeField] private bool useIK;
    [SerializeField] private bool useStaticSegments = true; 
    [SerializeField] private int staticSegments = 4;
    [SerializeField] private int segmentsPerUnit = 4;
    [SerializeField] private int iterations = 10;
    [SerializeField] private float threshold = .1f;
    
    private Vector3 pole;

    public void SetSpring(SpringJoint spring)
    {
        springJoint = spring;
    }

    public void DestroySpring()
    {
        Destroy(springJoint);
    }

    private void Update()
    {
        if(springJoint == null || springJoint.connectedBody == null)
        {
            lineRenderer.enabled = false;
            if(collider) collider.enabled = false;
            return;
        }

        float width = GetWidth();; 

        // Update line
        UpdateLineRenderer(width);

        // Update collider
        UpdateCollider(width);
    }

    private float GetWidth()
    {
        if(dynamicWidth)
        {
            // Calculate width based on spring tension
            float p = springJoint.currentForce.magnitude / maxStretchForce;
            return Mathf.Lerp(lineWidthMinMax[0], lineWidthMinMax[1], 1 - p);
        }

        return defaultWidth;
    }

    private void UpdateLineRenderer(float width)
    {
        if(drawLine == false) return;
    
        lineRenderer.enabled = true;

        // Update width
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    
        // Get positions
        Vector3 startPos = springJoint.transform.position + springJoint.anchor;
        Vector3 endPos = springJoint.connectedBody.position + springJoint.connectedAnchor;
        
        // Set positions
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Use IK to simulate a rope with no tension
        if(useIK)
        {
            float distance = Vector3.Distance(startPos, endPos);

            if (distance < springJoint.maxDistance)
            {
                // Number of segments per unit + 1
                //    1    2    3     Segments
                // 1    2    3    4   Points
                // |----|----|----|
                // 0..............1   Units    
                int points = useStaticSegments? staticSegments + 1: Mathf.FloorToInt(distance * segmentsPerUnit) + 1;
                float segmentLength = distance / points;

                Vector3[] positions = new Vector3[points];
                positions[0] = startPos;

                for(int i = 0; i < iterations; i++)
                {
                    // Iterate backwards from target
                    positions[points-1] = endPos; // Last pos
                    for (int j = positions.Length - 2; j > 0; j--)
                    {
                        Vector3 dirToPrev = (positions[j] - positions[j+1]).normalized;
                        positions[j] = positions[j+1] + dirToPrev * segmentLength;
                    }

                    // Iterate forward
                    for (int j = 1; j < positions.Length - 1; j++)
                    {
                        Vector3 dirToNext = (positions[j] - positions[j-1]).normalized;
                        positions[j] = positions[j-1] + dirToNext * segmentLength;
                    }

                    // Close enough
                    if(Vector3.Distance(positions[positions.Length-1], endPos) <= threshold)
                        break;
                }

                // Pole always under midpoint of rope
                Vector3 mid = Vector3.Lerp(startPos, endPos, .5f);
                pole = startPos - Vector3.up;

                for (var i = 1; i < positions.Length - 1; i++)
                {
                    Plane plane = new Plane(positions[i+1] - positions[i-1], positions[i-1]);
                    var projectedPole = plane.ClosestPointOnPlane(pole);
                    var projectedPos =  plane.ClosestPointOnPlane(positions[i]);
                    var angle = Vector3.SignedAngle(projectedPos - positions[i-1], projectedPole - positions[i-1], plane.normal);
                    positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i-1]) + positions[i-1];
                }

                lineRenderer.positionCount = points;
                lineRenderer.SetPositions(positions);
            }
        }
    }

    private void UpdateCollider(float width)
    {
        if(enableCollisions == false || collider == null) return;

        Vector3 pos0 = springJoint.transform.position + springJoint.anchor;
        Vector3 pos1 = springJoint.connectedBody.position + springJoint.connectedAnchor;

        collider.enabled = true;
        collider.radius = width;
        collider.transform.position = Vector3.Lerp(pos0, pos1, .5f);
        collider.transform.rotation = Quaternion.LookRotation(pos1 - pos0);
        collider.height = Vector3.Distance(pos0, pos1);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawCube(pole, Vector3.one);
    }
}
