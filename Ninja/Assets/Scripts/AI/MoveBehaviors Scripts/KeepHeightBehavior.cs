using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Move Behavior/Keep Height Behavior")]
public class KeepHeightBehavior : MoveBehavior
{
    [SerializeField] private float height;

    public override Vector3 CalculateMove(Transform local, Transform target = null, List<Transform> flock = null)
    {
        Vector3 targetPosition = local.position;
        targetPosition.y = height;

        return targetPosition - local.position;
    }
}
