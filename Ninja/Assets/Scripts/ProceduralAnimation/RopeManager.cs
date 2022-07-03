using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeManager : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    private Mesh mesh;

    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private int segmentsPerUnit = 4;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private bool startFixed;
    [SerializeField] private bool endFixed;

    private float segLength;
    private List<GameObject> segments = new List<GameObject>();
    
    private void Start()
    {
        // Localize start and end position
        startPos = transform.position + startPos;
        endPos = transform.position + endPos;

        CreateRope(startFixed, endFixed);
    }

    private void CreateRope(bool startFixed = true, bool endFixed = false)
    {
        float dist = Vector3.Distance(endPos, startPos);
        Vector3 dir = (endPos - startPos).normalized;
        int segmentsNum = Mathf.FloorToInt(dist);
        segLength = dist / segmentsNum;

        Rigidbody lastRigidbody = null;

        // Create segments
        for(int i = 0; i < segmentsNum; i++)
        {
            GameObject go = Instantiate(segmentPrefab, startPos + dir * segLength * .5f + dir * segLength * i, Quaternion.identity, transform);
            segments.Add(go);

            Vector3 scale = go.transform.localScale;
            scale.y = segLength;
            go.transform.localScale = scale;

            Rigidbody rb = go.GetComponent<Rigidbody>();
            ConfigurableJoint cj = go.GetComponent<ConfigurableJoint>();

            if(i == 0 && startFixed || i == segmentsNum - 1 && endFixed)
                rb.isKinematic = true;

            cj.connectedBody = lastRigidbody;
            cj.autoConfigureConnectedAnchor = false;
            cj.anchor = new Vector3(0, segLength * .5f, 0);

            lastRigidbody = rb;
        }

    }

    private void LateUpdate() 
    {
        UpdateMesh();    
    }

    private void UpdateMesh()
    {
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
