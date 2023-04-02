using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrail : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    private List<Vector3> positions = new List<Vector3>();

    private enum TrailType {LineRenderer, TrailRenderer};
    [SerializeField] private TrailType type;                                                                             

    private void Update()
    {
        RaycastHit hit;
        bool raycast = Physics.Raycast(transform.position, transform.forward, out hit, 1f);

        switch(type)
        {
            case TrailType.LineRenderer:
                if(raycast)
                    positions.Add(hit.point);
                lineRenderer.positionCount = positions.Count;
                lineRenderer.SetPositions(positions.ToArray());
                break;
            case TrailType.TrailRenderer:
                trailRenderer.emitting = raycast;
                break;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawRay(transform.position, transform.forward); 
    }
}
