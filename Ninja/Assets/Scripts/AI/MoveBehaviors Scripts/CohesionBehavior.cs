using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Move Behavior/Cohesion Move Behavior")]
public class CohesionBehavior : MoveBehavior
{
    public override Vector3 CalculateMove(Transform local, Transform target = null, List<Transform> flock = null)
    {
        if(flock.Count == 0)
            return Vector3.zero;

        Vector3 cohesionMove = Vector3.zero;

        // Find average position
        foreach(Transform item in flock)
            cohesionMove += item.position;
        
        cohesionMove /= flock.Count;

        // Offset local position
        cohesionMove -= local.position;
        Debug.Log("Cohesion Move: " + cohesionMove);
        return cohesionMove;
    }
}
