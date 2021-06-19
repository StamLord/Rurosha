using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveBehavior : ScriptableObject
{
    public abstract Vector3 CalculateMove(Transform local, Transform target = null, List<Transform> flock = null);

    public Vector3 CalculateMoveFlat(Transform local, Transform target = null, List<Transform> flock = null)
    {
        Vector3 move = CalculateMove(local, target, flock);
        move.y = 0;

        return move;
    }
}
