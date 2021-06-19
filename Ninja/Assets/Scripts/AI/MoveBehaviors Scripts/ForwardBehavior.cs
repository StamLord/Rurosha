using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Move Behavior/Forward Behavior")]
public class ForwardBehavior : MoveBehavior
{
    public override Vector3 CalculateMove(Transform local, Transform target = null, List<Transform> flock = null)
    {
        return local.forward;
    }
}
