using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Move Behavior/Random Move Behavior")]
public class RadomBehavior : MoveBehavior
{
    public override Vector3 CalculateMove(Transform local, Transform target = null, List<Transform> flock = null)
    {
        return Random.insideUnitSphere;
    }
}
