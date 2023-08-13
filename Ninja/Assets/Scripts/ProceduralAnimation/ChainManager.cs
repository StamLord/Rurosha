using System.Collections;
using UnityEngine;

public class ChainManager : MonoBehaviour
{
    [SerializeField] private int segmentsPerUnit = 4;
    [SerializeField] private ChainLink linkPrefab;
    [SerializeField] private bool drawLine;
    [SerializeField] private LineRenderer lineRenderer;

    private ChainLink[] links;

    public void InitializeChain(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end); 
        int segments = Mathf.FloorToInt(distance * segmentsPerUnit);
        float segmentLength = distance / segments;
        links = new ChainLink[segments];

        for (int i = 0; i < segments; i++)
        {
            Vector3 position = Vector3.Lerp(start, end, (float)i / (segments - 1));
            links[i] = Instantiate(linkPrefab, position, Quaternion.identity, transform);
            links[i].SpringJoint.maxDistance = segmentLength;

            if(i == 0)
                links[i].DestroySpringJoint();
            else
                links[i].ConnectLink(links[i-1]);
        }
    }

    public ChainLink this[int index] 
    {
        get => links[index];
    }

    public int Count => links.Length;

    private void Update()
    {
        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if(drawLine == false || lineRenderer == null || links == null) return;

        lineRenderer.positionCount = links.Length;

        for (int i = 0; i < links.Length; i++)
            lineRenderer.SetPosition(i, links[i].transform.position);
    }
}
