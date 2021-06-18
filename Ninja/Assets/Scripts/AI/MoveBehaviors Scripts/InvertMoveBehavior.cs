using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Move Behavior/Invert Move Behavior")]
public class InvertMoveBehavior : MoveBehavior
{
    [SerializeField] private MoveBehavior behavior;

    public override Vector3 CalculateMove(Transform local, Transform target = null, List<Transform> flock = null)
    {
        return behavior.CalculateMove(local, target, flock) * -1;
    }
}
