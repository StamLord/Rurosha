using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveBehavior : ScriptableObject
{
    public abstract Vector3 CalculateMove(Transform local, Transform target = null, List<Transform> flock = null);
}
