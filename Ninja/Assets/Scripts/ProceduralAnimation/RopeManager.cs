using System.Collections.Generic;
using UnityEngine;

public class RopeManager : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh mesh;

    [Header("Rope Settings")]
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private float segmentsPerUnit = 4;

    [Header("Prefab")]
    [SerializeField] private GameObject segmentPrefab;

    [Header("Segment Settings")]
    [SerializeField] private bool startFixed;
    [SerializeField] private bool endFixed;
    [SerializeField] private bool scaleToFit;
    
    [Header("Joint Configuration")]
    [SerializeField] private Rigidbody firstSegment;
    [SerializeField] private bool autoConfigureConnectedAnchor;
    [SerializeField] private bool useCustomAnchor;
    [SerializeField] private Vector3 anchor;
    [SerializeField] private bool useCustomConnectedAnchor;
    [SerializeField] private Vector3 connectedAnchor;

    private List<GameObject> segments = new List<GameObject>();
    private float segmentLength;

    private void Start()
    {
        CreateRope(startFixed, endFixed);
    }

    private void OnValidate() 
    {
        UpdateAllSegments();
    }

    private void CreateRope(bool startFixed = true, bool endFixed = false)
    {
        // Get worldposition of start and end position
        startPos = transform.position + startPos;
        endPos = transform.position + endPos;

        // Get distance and direction from start to end
        float dist = Vector3.Distance(endPos, startPos);
        Vector3 dir = (endPos - startPos).normalized;

        // Calculate amount and length of segments
        int segmentsNum = Mathf.FloorToInt(dist * segmentsPerUnit);
        segmentLength = dist / segmentsNum;

        // Create segments
        for(int i = 0; i < segmentsNum; i++)
        {
            GameObject go = Instantiate(segmentPrefab, startPos + dir * segmentLength * .5f + dir * segmentLength * i, Quaternion.identity, transform);
            segments.Add(go);
        }

        UpdateAllSegments();
    }

    private void UpdateAllSegments()
    {
        Rigidbody lastRigidbody = firstSegment;

        for(int i = 0; i < segments.Count; i++)
        {
            Rigidbody rb = segments[i].GetComponent<Rigidbody>();

            // Set extremeties to kinematic
            if(i == 0 && startFixed || i == segments.Count - 1 && endFixed)
                rb.isKinematic = true;
            
            UpdateSegment(segments[i], lastRigidbody);

            // Prepare for next iteration
            lastRigidbody = rb;
        }
    }

    private void UpdateSegment(GameObject gameObject, Rigidbody connectedRigidbody)
    {
        // Stretch segment to fit between other segments. Used in case of generic meshes like a cube to create a rope
        if(scaleToFit)
        {
            Vector3 scale = gameObject.transform.localScale;
            scale.y = segmentLength;
            gameObject.transform.localScale = scale;
        }

        // Set joint
        ConfigureJoint(gameObject, connectedRigidbody, segmentLength);
    }

    protected virtual void ConfigureJoint(GameObject gameObject, Rigidbody rigidbody, float segmentLength)
    {
        ConfigurableJoint cj = gameObject.GetComponent<ConfigurableJoint>();
        if(cj == null) return;

        cj.connectedBody = rigidbody;
        
        cj.autoConfigureConnectedAnchor = autoConfigureConnectedAnchor;
        
        if(autoConfigureConnectedAnchor == false)
        {
            // Set anchor point
            if(useCustomAnchor)
                cj.anchor = anchor;
            else
                cj.anchor = new Vector3(0, segmentLength * .5f, 0);

            // Set connected anchor point
            if(useCustomConnectedAnchor)
                cj.connectedAnchor = connectedAnchor;
            else
                cj.anchor = new Vector3(0, -segmentLength * .5f, 0);
        }
    }

    private void LateUpdate() 
    {
        UpdateMesh();    
    }

    private void UpdateMesh()
    {
        if(meshFilter == null) return;

        if(mesh == null)
        {
            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;

            Vector3[] vertex = new Vector3[segments.Count * 4 * 6];
            int[] triangles = new int[vertex.Length];
        }
    }
    
    private void OnDrawGizmos() 
    {
        Gizmos.DrawSphere(transform.position + startPos, .2f);    
        Gizmos.DrawSphere(transform.position + endPos, .2f);
    }
}
