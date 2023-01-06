using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtEntity : MonoBehaviour
{
    [SerializeField] private LayerMask followLayers;
    [SerializeField] private float turnSpeed = .2f;
    [SerializeField] private Transform origin;
    [SerializeField] private float radius;
    [SerializeField] private bool invertForward;
    [SerializeField] private float horizontalVisionAngle = 45f;
    [SerializeField] private float verticalVisionAngle = 20f;
    [SerializeField] private LayerMask blockVisionMask;

    [SerializeField] private bool debugView;

    private Vector3 originalPos;

    private void Start() 
    {
        originalPos = transform.position;    
    }

    private void Update()
    {
        // Find relevant colliders
        Collider[] colliders = Physics.OverlapSphere(origin.position, radius, followLayers);

        // Filter colliders that are not in our sight cone or blocked by obstacles
        List<Collider> inSight = new List<Collider>();
        foreach(Collider c in colliders)
        {
            if(IsLineOfSight(c.transform.position))
                inSight.Add(c);
        }
        
        // Set back to origin if no colliders
        if(inSight.Count < 1)
        {
            transform.position = Vector3.Lerp(transform.position, originalPos, turnSpeed * Time.deltaTime);
            return;
        }

        // Find closest collider
        Collider closest = colliders[0];
        float distance = Mathf.Infinity;

        foreach(Collider c in inSight)
        {
            float d = Vector3.Distance(transform.position, c.transform.position);
            if(d < distance)
            {
                closest = c;
                distance = d;
            }
        }

        // Position at closest collider;
        transform.position = Vector3.Lerp(transform.position, closest.transform.position, turnSpeed * Time.deltaTime);
    }

    private bool IsLineOfSight(Vector3 position)
    {
        Vector3 direction = position - origin.position;

        // Split angles for vertical / horizontal

        // [Vertical] Get angle around X axis - Angle between direction to target and a "flattned" direction with y component = 0
        Vector3 flatYDir = new Vector3(direction.x, 0, direction.z);
        float angleX = Vector3.Angle(direction, flatYDir);
        
        if(angleX > verticalVisionAngle) return false;

        // [Horizontal] Get angle around Y axis - We flatten both us and target on y axis ( y = 0) and get angle between them
        Vector3 tPos = new Vector3(position.x, 0, position.z);
        Vector3 lPos = new Vector3(origin.position.x, 0, origin.position.z);
        Vector3 dir2 = tPos - lPos;
        float angleY = (invertForward)? Vector3.Angle(-origin.forward, dir2) : Vector3.Angle(origin.forward, dir2);

        if(angleY > horizontalVisionAngle) return false;

        // Check line of sight
        RaycastHit hit;
        bool blocked = Physics.Raycast(origin.position, direction, out hit, direction.magnitude, blockVisionMask);
        return (blocked == false);
    }

    private void OnDrawGizmosSelected()
    {
        if(debugView == false) return;
        
        // Vision Cone Representation
        Color coneColor = Color.blue;
        Color radiusColor = Color.blue;
        radiusColor.a = .5f;
        
        Gizmos.color = radiusColor;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = coneColor;
        if(invertForward)
        {
            Gizmos.DrawLine(origin.position, origin.position + Quaternion.AngleAxis(horizontalVisionAngle, transform.up) * -origin.forward * radius);
            Gizmos.DrawLine(origin.position, origin.position + Quaternion.AngleAxis(-horizontalVisionAngle, transform.up) * -origin.forward * radius);
            Gizmos.DrawLine(origin.position, origin.position + Quaternion.AngleAxis(verticalVisionAngle, transform.right) * -origin.forward * radius);
            Gizmos.DrawLine(origin.position, origin.position + Quaternion.AngleAxis(-verticalVisionAngle, transform.right) * -origin.forward * radius);
        }
        else
        {
            Gizmos.DrawLine(origin.position, origin.position + Quaternion.AngleAxis(horizontalVisionAngle, transform.up) * origin.forward * radius);
            Gizmos.DrawLine(origin.position, origin.position + Quaternion.AngleAxis(-horizontalVisionAngle, transform.up) * origin.forward * radius);
            Gizmos.DrawLine(origin.position, origin.position + Quaternion.AngleAxis(verticalVisionAngle, transform.right) * origin.forward * radius);
            Gizmos.DrawLine(origin.position, origin.position + Quaternion.AngleAxis(-verticalVisionAngle, transform.right) * origin.forward * radius);
        }
        
    }
}
